using System;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;

//using Newtonsoft.Json;

namespace EventBus.Base.Events
{
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

        protected virtual string ProcessEventName(string eventName)
        {
            if (EventBusConfig.DeleteEventPrefix)
            {
                eventName = eventName.TrimStart(EventBusConfig.EventNamePrefix.ToArray());
            }

            if (EventBusConfig.DeleteEventSuffix)
            {
                eventName = eventName.TrimEnd(EventBusConfig.EventNameSuffix.ToArray());
            }

            return eventName;
        }

        protected virtual string GetSubName(string eventName)
        {
            return $"{EventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }

        public virtual void Dispose()
        {
            EventBusConfig = null;
            SubsManager.Clear();
        }

        protected async Task<bool> ProcessEvent(string eventName, string message)
        {
            try
            {
                eventName = ProcessEventName(eventName);
                if (!SubsManager.HasSubscriptionForEvent(eventName))
                {
                    return false;
                }

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

                        await InvokeHandlerAsync(handleMethod, handler, integrationEvent);

                        _logger.LogInformation($"Event handled successfully: {eventType.Name}");
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                LogAndRethrowException(e);
            }

            return false;
        }

        public object DeserializeEvent(string message, Type eventType)
        {
            try
            {
                var eventJson = "{\"Id\":1,\"CreatedDate\":\"2023-08-24T15:50:22.6561522+04:00\"}";
                
                //var y = JsonConvert.DeserializeObject(message, eventType);
                

                var x = JsonSerializer.Deserialize(message, eventType);
                
                return x;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error deserializing event of type: {eventType.Name}");
                throw;
            }
        }

        private async Task InvokeHandlerAsync(MethodInfo handleMethod, object handler, object integrationEvent)
        {
            try
            {
                await (Task)handleMethod.Invoke(handler, new object[] { integrationEvent });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling event: {integrationEvent.GetType().Name}");
                throw;
            }
        }

        private void LogAndRethrowException(Exception e)
        {
            _logger.LogError(e, "An exception occurred while processing the event.");
        }


        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}