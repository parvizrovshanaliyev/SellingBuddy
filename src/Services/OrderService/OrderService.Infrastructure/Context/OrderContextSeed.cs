using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using OrderService.Domain.SeedWork;
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
        await using (context)
        {
            await context.Database.MigrateAsync(); // Apply any pending migrations
            
            if (!context.CardTypes.Any())
            {
                
                var cardTypes = Enumeration.GetAll<CardType>();
                
                context.CardTypes.AddRange(
                    cardTypes
                );
                
                await context.SaveChangesAsync();
            }
            
            if (!context.OrderStatuses.Any())
            {
                var orderStatuses = Enumeration.GetAll<OrderStatus>();
                
                context.OrderStatuses.AddRange(
                    orderStatuses
                );
                
                await context.SaveChangesAsync();
            }
        }
    }
}