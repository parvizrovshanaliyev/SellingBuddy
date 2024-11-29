using Web.ApiGateway.Services.Interfaces;

namespace Web.ApiGateway.Services;

public static class ConfigureServices
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IBasketService, BasketService>();

        return services;
    }
}