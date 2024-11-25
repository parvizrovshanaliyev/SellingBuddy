using System.Reflection;
using Api.Shared.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;

namespace CatalogService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSharedAppSettingsAndEnvironmentVariables(args);

        builder.Services.AddServices(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);

        builder.Services.AddPersistence(builder.Configuration);
        
        builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection("CatalogSettings"));

        var app = builder.Build();

        app.UseServices(builder.Configuration, app.Lifetime, app.Environment);

        app.MigrateDbContext<CatalogDbContext>((context, services) =>
        {
            // Seed data if needed
            CatalogContextSeed
                .SeedAsync(context, services.GetRequiredService<ILogger<CatalogContextSeed>>())
                .Wait();
        });

        app.Run();
    }
}