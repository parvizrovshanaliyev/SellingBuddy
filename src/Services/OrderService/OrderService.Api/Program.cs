using System.Reflection;
using Api.Shared.Extensions;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderService.Api.IntegrationEvents;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Infrastructure.Context;

namespace OrderService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSharedAppSettingsAndEnvironmentVariables(args);
        
        builder.Services.AddServices(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);
        
        builder.Services.AddApplication();
        
        builder.Services.AddPersistence(builder.Configuration);
        
        builder.Services.AddIntegrationEvents(Assembly.GetExecutingAssembly().GetName().Name, builder.Configuration);
        
        var app = builder.Build();

        app.UseServices(builder.Configuration, app.Lifetime, app.Environment);
        
        app.MigrateDbContext<OrderDbContext>((context, services) =>
        {
            // Seed data if needed
            OrderContextSeed
                .SeedAsync(context, services.GetRequiredService<ILogger<OrderContextSeed>>())
                .Wait();
        });

        app.Run();
    }
}