using Microsoft.Extensions.DependencyInjection;
using WebApp.Application.Services;
using WebApp.Application.Services.Identity;
using WebApp.Application.Services.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureService
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IBasketService, BasketService>();
        services.AddScoped<IOrderService, OrderService>();
        
        return services;
    }
}