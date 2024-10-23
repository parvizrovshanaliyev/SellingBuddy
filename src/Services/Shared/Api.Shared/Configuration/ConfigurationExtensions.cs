using Microsoft.Extensions.Configuration;

namespace Api.Shared.Configuration;

public static  class ConfigurationExtensions
{
    public static string GetAspNetCoreUrls(this IConfiguration configuration)
    {
        var urls = configuration["ASPNETCORE_URLS"] 
                   ?? configuration["Kestrel:Endpoints:Http:Url"];
    
        return urls;
    }
}