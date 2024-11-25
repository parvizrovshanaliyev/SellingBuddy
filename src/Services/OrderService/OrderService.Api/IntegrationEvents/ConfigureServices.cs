using System.Reflection;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Api.IntegrationEvents.EventHandlers;
using OrderService.Api.IntegrationEvents.Events;

namespace OrderService.Api.IntegrationEvents;

public static class ConfigureServices
{
    public static IServiceCollection AddIntegrationEvents(this IServiceCollection services, string serviceName, IConfiguration configuration)
    {
        services.AddTransient<OrderCreatedIntegrationEventHandler>();
        
        services.AddSingleton(sp =>
            EventBusFactory.Create(
                EventBusConfig.GetRabbitMQConfig(serviceName, configuration),
                sp));
        
        var eventBus = services.BuildServiceProvider().GetService<IEventBus>();
        
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        return services;
    }
}