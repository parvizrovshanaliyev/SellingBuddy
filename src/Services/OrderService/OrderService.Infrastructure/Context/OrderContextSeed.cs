using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;

namespace OrderService.Infrastructure.Context;

public class OrderContextSeed
{
    public static async Task SeedAsync(OrderDbContext context, ILogger<OrderContextSeed> logger)
    {
        var policy = Policy.Handle<SqlException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning($"Error occurred, retrying (attempt {retryCount})");
                }
            );

        await policy.ExecuteAsync(() => ProcessSeeding(context, logger));
    }

    private static async Task ProcessSeeding(OrderDbContext context, ILogger<OrderContextSeed> logger)
    {
        await Task.CompletedTask;
        return;
        //await context.Database.MigrateAsync(); // Apply any pending migrations

        // if (!context.CatalogBrands.Any())
        // {
        //     context.CatalogBrands.AddRange(
        //         new CatalogBrand { Brand = "Brand 1" },
        //         new CatalogBrand { Brand = "Brand 2" },
        //         new CatalogBrand { Brand = "Brand 3" }
        //     );
        //     await context.SaveChangesAsync();
        // }
        //
        // if (!context.CatalogTypes.Any())
        // {
        //     context.CatalogTypes.AddRange(
        //         new CatalogType { Type = "Type 1" },
        //         new CatalogType { Type = "Type 2" },
        //         new CatalogType { Type = "Type 3" }
        //     );
        //     await context.SaveChangesAsync();
        // }
        //
        // if (!context.CatalogItems.Any())
        // {
        //     context.CatalogItems.AddRange(
        //         new CatalogItem
        //         {
        //             Name = "Item 1",
        //             Description = "Description 1",
        //             Price = 10.99m,
        //             PictureFileName = "item1.jpg",
        //             PictureUri = "https://example.com/item1.jpg",
        //             CatalogTypeId = 1, // Use the appropriate type ID
        //             CatalogBrandId = 1 // Use the appropriate brand ID
        //         }
        //         // Add more items as needed
        //     );
        //     await context.SaveChangesAsync();
        // }
    }
}