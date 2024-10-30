using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure;

public static class ConfigureServices
{
    // add persistence
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        // Default connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<OrderDbContext>(options =>
            options.UseSqlServer(connectionString,
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(OrderDbContext).GetTypeInfo().Assembly.GetName().Name);
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                }));

        services.AddScoped<IOrderRepository, OrderRepository>();

        return services;
    }
}