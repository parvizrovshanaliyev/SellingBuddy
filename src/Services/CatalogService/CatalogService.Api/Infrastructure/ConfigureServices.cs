﻿using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CatalogService.Api.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseSqlServer(connectionString, builder =>
            {
#if DEBUG
                options.EnableDetailedErrors(); // To get field-level error details
                options.EnableSensitiveDataLogging(); // To get parameter values - don't use this in production
                options.ConfigureWarnings(warningAction =>
                {
                    warningAction.Log(CoreEventId.FirstWithoutOrderByAndFilterWarning,
                        CoreEventId.RowLimitingOperationWithoutOrderByWarning);
                });
#endif
                builder.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(30),
                    null
                );
            });

            options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())); // Add console logger
        });

        return services;
    }
}