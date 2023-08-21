using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CatalogService.Api.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddPersistent(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        try
        {
            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlServer(connectionString, builder =>
                {
#if DEBUG
                    options.EnableDetailedErrors(); // To get field-level error details
                    options.EnableSensitiveDataLogging(); // To get parameter values - don't use this in production
                    options.ConfigureWarnings(warningAction =>
                    {
                        warningAction.Log(new EventId[]
                        {
                            CoreEventId.FirstWithoutOrderByAndFilterWarning,
                            CoreEventId.RowLimitingOperationWithoutOrderByWarning
                        });
                    });
#endif
                    builder.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                });

                options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())); // Add console logger
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return services;
    }
}