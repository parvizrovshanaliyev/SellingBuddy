using Microsoft.Extensions.DependencyInjection;
using WebApp.Application.Services;
using WebApp.Application.Services.Catalog;
using WebApp.Application.Services.Identity;
using WebApp.Infrastructure.Indentity;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureService
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogService, CatalogService>();
        return services;
    }
}