using System.Reflection;
using Api.Shared.Caching;
using Api.Shared.Extensions;
using BasketService.Api.Core.Infrastructure;
using BasketService.Api.Core.Infrastructure.Repository;
using BasketService.Api.Core.Infrastructure.Services;
using BasketService.Api.IntegrationEvents;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace BasketService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSharedAppSettingsAndEnvironmentVariables(args);
        
        builder.Services.AddServices(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);

        builder.Services.AddInfrastructure(builder.Configuration);
        
        builder.Services.AddIntegrationEvents(Assembly.GetExecutingAssembly().GetName().Name, builder.Configuration);

        var app = builder.Build();

        app.UseServices(builder.Configuration, app.Lifetime, app.Environment);

        app.Run();
    }
}