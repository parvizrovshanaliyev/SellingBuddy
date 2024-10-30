using Microsoft.Extensions.Configuration;

namespace Api.Shared.Extensions;

public static  class ConfigurationExtensions
{
    public static string GetAspNetCoreUrls(this IConfiguration configuration)
    {
        var urls = configuration["ASPNETCORE_URLS"] 
                   ?? configuration["Kestrel:Endpoints:Http:Url"];
    
        return urls;
    }
    
    public static string GetDbConnectionStringFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory) // This will look in the output directory
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        return configuration.GetConnectionString("DefaultConnection");
    }
}