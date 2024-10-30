using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Api.Shared.Auth;
using Api.Shared.Caching;
using Api.Shared.Consul;
using Api.Shared.Extensions;
using Api.Shared.Host;
using Api.Shared.Swagger;
using BasketService.Api.Core.Infrastructure.Repository;
using BasketService.Api.Core.Infrastructure.Services;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BasketService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSharedAppSettingsAndEnvironmentVariables(args);
        
        builder.Services.AddServices(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);

        builder.Services.AddCaching(builder.Configuration);
        
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        
        builder.Services.AddScoped<IBasketRepository , BasketRepository>();
        
        builder.Services.AddSingleton(sp =>
            EventBusFactory.Create(
                EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name,builder.Configuration),
                sp));
        
        var eventBus = builder.Services.BuildServiceProvider().GetService<IEventBus>();
        
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        var app = builder.Build();

        app.UseServices(builder.Configuration, app.Lifetime, app.Environment);

        app.Run();
    }
}