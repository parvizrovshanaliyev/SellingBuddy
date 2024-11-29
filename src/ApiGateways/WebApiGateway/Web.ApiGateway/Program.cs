using System.Reflection;
using Api.Shared.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Web.ApiGateway.Configurations;
using Web.ApiGateway.Infrastructure.HttpClient;
using Web.ApiGateway.Services;

namespace Web.ApiGateway;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Kestrel and Content Root
        builder.Host.UseContentRoot(Directory.GetCurrentDirectory());

        // Configure app configuration to load multiple Ocelot JSON files
        builder.Host.ConfigureAppConfiguration((context, config) =>
        {
            // Console.WriteLine($"Environment: {context.HostingEnvironment.EnvironmentName}");
            // Console.WriteLine($"Content Root: {context.HostingEnvironment.ContentRootPath}");
            //
            // config
            //     .SetBasePath(context.HostingEnvironment.ContentRootPath)
            //     .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //     .AddJsonFile("SharedAppSettings.json", optional: true, reloadOnChange: true)
            //     .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true,
            //         reloadOnChange: true)
            config
                .SetBasePath(context.HostingEnvironment.ContentRootPath)
                .AddJsonFile($"ocelot.{context.HostingEnvironment.EnvironmentName}.json", optional: true,
                    reloadOnChange: true);

            config.AddEnvironmentVariables();
        });
        
        builder.AddSharedAppSettingsAndEnvironmentVariables(args);

        // Configure logging
        builder.Logging.AddConsole();

        if (IsDebugMode())
        {
            // Add Ocelot services, Consul, and Polly for resilience
            builder.Services.AddOcelot(builder.Configuration);
        }
        else
        {
            // Add Ocelot services, Consul, and Polly for resilience
            builder.Services.AddOcelot(builder.Configuration)
                .AddConsul()
                .AddPolly();
        }

        // Add Swagger for Ocelot
        builder.Services.AddSwaggerForOcelot(builder.Configuration);

        // Add Controllers
        builder.Services.AddControllers();

        // Add API Explorer for generating Swagger UI endpoints
        builder.Services.AddEndpointsApiExplorer();

        // Add SwaggerGen to generate Swagger UI for the API Gateway itself
        builder.Services.AddSwaggerDocumentation(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);

        builder.Services.AddCors(builder.Configuration);

        builder.Services.AddHttpClients(builder.Configuration);

        builder.Services.AddAppServices();
        
        builder.Services.AddAuthService(builder.Configuration);

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

        app.UseAuthentication();
        app.UseAuthorization();

        // Map controller endpoints
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        // Run Ocelot middleware
        app.UseOcelot().GetAwaiter().GetResult();

        // Run the application
        app.Run();
    }

    public static bool IsDebugMode()
    {
#if DEBUG
        return true;
#else
        return false;
#endif
    }
}