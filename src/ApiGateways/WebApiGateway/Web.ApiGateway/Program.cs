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
        
        // Specify the folder containing Ocelot routes configuration
        var routes = builder.Configuration.GetSection("OcelotConfig:Directory").Get<string>();

        Console.WriteLine($"Ocelot routes configuration from SharedAppSettings: {routes}");

        // Configure Ocelot with Swagger support
        builder.Configuration.AddOcelotWithSwaggerSupport(options =>
        {
            options.Folder = routes;
        });

        // Add Ocelot services and Polly for resilience
        builder.Services.AddOcelot(builder.Configuration).AddPolly().AddConsul();


        builder.Host.ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile($"ocelot.json", optional: true, reloadOnChange: true);
            config.AddJsonFile($"ocelot.Docker.json", optional: true, reloadOnChange: true);

            config.AddEnvironmentVariables();
        });

        // Add health checks
        //builder.Services.ConfigureHealthChecks(builder.Configuration);

        //builder.Services.AddAuthWithJwtTokenService(builder.Configuration);

        // Add Swagger for Ocelot
        builder.Services.AddSwaggerForOcelot(builder.Configuration);

        builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
            .AddOcelot(routes, builder.Environment)
            .AddEnvironmentVariables();

        // Add Controllers
        builder.Services.AddControllers();

        // Add API Explorer for generating Swagger UI endpoints
        builder.Services.AddEndpointsApiExplorer();

        // Add SwaggerGen to generate Swagger UI for the API Gateway itself
        builder.Services.AddSwaggerGen();

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
