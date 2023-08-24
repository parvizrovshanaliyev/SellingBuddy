using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.Base.Events;

public abstract class BaseEventBus : IEventBus
{
    #region fields

    private readonly IServiceProvider _serviceProvider;
    protected readonly IEventBusSubscriptionManager SubsManager;
    public EventBusConfig EventBusConfig { get; set; }
    private readonly ILogger _logger;

    #endregion
    
    #region ctor

    protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig)
    {
        _serviceProvider = serviceProvider;
        SubsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        EventBusConfig = eventBusConfig;

        _logger = serviceProvider.GetService(typeof(ILogger<BaseEventBus>)) as ILogger<BaseEventBus>;
    }

    #endregion

    public virtual void Dispose()
    {
        EventBusConfig = null;
        SubsManager.Clear();
    }

    public object DeserializeEvent(string message, Type eventType)
    {
        var obj = JsonSerializer.Deserialize(message, eventType);

        return obj;
    }

    public abstract void Publish(IntegrationEvent @event);

    public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

    public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

    protected virtual string ProcessEventName(string eventName)
    {
        if (EventBusConfig.DeleteEventPrefix) eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());

        if (EventBusConfig.DeleteEventSuffix) eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());

        return eventName;
    }

    protected virtual string GetSubName(string eventName)
    {
        return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
    }

    protected async Task<bool> ProcessEvent(string eventName, string message)
    {
        try
        {
            eventName = ProcessEventName(eventName);
            
            if (!SubsManager.HasSubscriptionForEvent(eventName)) return false;

            var subscriptions = SubsManager.GetHandlersForEvent(eventName);

            using (var scope = _serviceProvider.CreateScope())
            {
                foreach (var subscription in subscriptions)
                {
                    var handler = _serviceProvider.GetService(subscription.HandlerType);

                    if (handler == null)
                    {
                        _logger.LogWarning($"Handler not available for subscription: {subscription.HandlerType}");
                        continue;
                    }

                    var eventType = SubsManager.GetEventTypeByName(
                        $"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");

                    var integrationEvent = DeserializeEvent(message, eventType);

                    var concreteHandlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                    var handleMethod = concreteHandlerType.GetMethod("Handle");

                    await (Task)handleMethod.Invoke(handler, new[] { integrationEvent });

                    _logger.LogInformation($"Event handled successfully: {eventType.Name}");
                }

                return true;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An exception occurred while processing the event.");
        }

        return false;
    }
}