using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace OrderService.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddAutoMapper(assembly);
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        
        return services;
    }
}