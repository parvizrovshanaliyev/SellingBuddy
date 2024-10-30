using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;

namespace Api.Shared.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder MigrateDbContext<TContext>(
        this IApplicationBuilder app,
        Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        using var scope = app.ApplicationServices.CreateScope();

        var services = scope.ServiceProvider;

        var logger = services.GetService<ILogger<TContext>>();

        var context = services.GetRequiredService<TContext>();

        try
        {
            logger!.LogInformation("Migrating database associated with context");

            var retryIntervals = new[]
            {
                TimeSpan.FromSeconds(3),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(6)
            };

            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetry(retryIntervals);

            retryPolicy.Execute(() => InvokeSeeder(seeder, context, services));

            logger!.LogInformation("Migrating database associated with context");
        }
        catch (Exception ex)
        {
            logger!.LogError(ex, $"An error occurred while migrating or seeding the {typeof(TContext).Name} database.");
        }

        return app;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context,
        IServiceProvider services) where TContext : DbContext
    {
        //context.Database.EnsureCreated();
        context.Database.Migrate(); // Apply migrations

        seeder(context, services); // Seed data
    }
}