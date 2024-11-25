using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BasketService.Api.IntegrationEvents;

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