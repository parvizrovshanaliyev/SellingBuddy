using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Shared.Configuration;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddSharedAppSettingsAndEnvironmentVariables(
        this WebApplicationBuilder builder,
        string[]? args = null)
    {
        IWebHostEnvironment env = builder.Environment;
        
        var envContentRootPath = env.ContentRootPath;

        Console.WriteLine("Add SharedAppSettings and Environment Variables: " + env.EnvironmentName);

        builder.Configuration.AddSharedConfiguration(envContentRootPath, env.EnvironmentName);

        builder.Configuration.AddEnvironmentVariables();

        // If args is provided, add command line arguments as configuration
        if (args != null)
        {
            Console.WriteLine("Add command line arguments as configuration" + string.Join(" ", args));
            //builder.Configuration.AddCommandLine(args);
        }

        builder.Services.AddHttpContextAccessor();

        return builder;
    }
    
    public static IConfigurationBuilder AddSharedConfiguration(this IConfigurationBuilder configurationBuilder, string contentRootPath, string environmentName)
    {
        var sharedFolder = Path.Combine(contentRootPath, "..", "..", "..", "SharedFiles");

        configurationBuilder
            .AddJsonFile(Path.Combine(sharedFolder, "SharedAppSettings.json"), optional: true)
            .AddJsonFile(Path.Combine(sharedFolder, $"SharedAppSettings.{environmentName}.json"), optional: true)
            .AddJsonFile("SharedAppSettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"SharedAppSettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

        return configurationBuilder;
    }
}
