using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;
using Ocelot.Provider.Consul;
using Web.ApiGateway.Configurations;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure Kestrel and Content Root
        builder.Host.UseContentRoot(Directory.GetCurrentDirectory());
        
        var ocelotConfigPath= string.Empty;
        // // Specify the folder containing Ocelot routes configuration
        var ocelotConfigDirectory = builder.Configuration.GetSection("OcelotConfig:DirectoryLocal").Get<string>();
        
        // Configure app configuration to load multiple Ocelot JSON files
        builder.Host.ConfigureAppConfiguration((context, config) =>
        {
            Console.WriteLine($"Environment: {context.HostingEnvironment.EnvironmentName}");
            Console.WriteLine($"Content Root: {context.HostingEnvironment.ContentRootPath}");
            
            config
                .SetBasePath(context.HostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                
            ocelotConfigPath = Path.Combine(context.HostingEnvironment.ContentRootPath, ocelotConfigDirectory);

            // Dynamically load all JSON files in OcelotConfig folder
            var jsonFiles = Directory.GetFiles(ocelotConfigPath, "*.json", SearchOption.AllDirectories);

            // Add each found JSON file to the Ocelot configuration
            foreach (var jsonFile in jsonFiles)
            {
                config.AddJsonFile(jsonFile, optional: false, reloadOnChange: true);
            }    
                
            config.AddEnvironmentVariables();
        });
        
        // Configure logging
        builder.Logging.AddConsole();
        
        // Configure Ocelot with Swagger support
        builder.Configuration.AddOcelotWithSwaggerSupport(options =>
        {
            options.Folder = ocelotConfigPath;
        });

        // Add Ocelot services, Consul, and Polly for resilience
        builder.Services.AddOcelot(builder.Configuration)
            .AddConsul()
            .AddPolly();

        // Add Swagger for Ocelot
        builder.Services.AddSwaggerForOcelot(builder.Configuration);
        
        // Add Controllers
        builder.Services.AddControllers();

        // Add API Explorer for generating Swagger UI endpoints
        builder.Services.AddEndpointsApiExplorer();

        // Add SwaggerGen to generate Swagger UI for the API Gateway itself
        builder.Services.AddSwaggerGen();
        
        builder.Services.AddCors(builder.Configuration);

        var app = builder.Build();

        // Enable Swagger UI
        app.UseSwagger();

        // Use Swagger UI for Ocelot
        app.UseSwaggerForOcelotUI(options =>
        {
            options.PathToSwaggerGenerator = "/swagger/docs";
            options.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
        });

        // Enable routing and authorization
        app.UseRouting();
        app.UseCorsPolicy();

        app.UseAuthorization();

        // Map controller endpoints
        app.UseEndpoints(endpoints => 
        { 
            endpoints.MapControllers(); 
        });

        // Run Ocelot middleware
        app.UseOcelot().GetAwaiter().GetResult();

        // Run the application
        app.Run();
    }
}
