using System;
using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;

namespace EventBus.Factory;

public static class EventBusFactory
{
    public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
    {
        // var conn= new DefaultServiceBusPersistentConnection(config)


        return config.EventBusType switch
        {
            EventBusType.AzureServiceBus => new EventBusServiceBus(config: config, serviceProvider: serviceProvider),
            _ => new EventBusRabbitMQ(config: config, serviceProvider: serviceProvider)
        };
    }
}