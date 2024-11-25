using Api.Shared.Caching;
using BasketService.Api.Core.Infrastructure.Repository;
using BasketService.Api.Core.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasketService.Api.Core.Infrastructure;

/// <summary>
///   Configure services
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Add infrastructure services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services ,IConfiguration configuration)
    {
        services.AddCaching(configuration);
        
        services.AddTransient<IIdentityService, IdentityService>();
        
        services.AddScoped<IBasketRepository , BasketRepository>();

        return services;
    }
}