using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Base.SubManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        #region fields

        protected readonly IServiceProvider ServiceProvider;
        protected readonly IEventBusSubscriptionManager SubsManager;
        public EventBusConfig EventBusConfig { get; set; }

        #endregion

        #region ctor

        protected BaseEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig)
        {
            ServiceProvider = serviceProvider;
            SubsManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
            EventBusConfig = eventBusConfig;
        }

        #endregion

        public virtual string ProcessEventName(string eventName)
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

        public virtual string GetSubName(string eventName)
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
            // Preprocess the event name
            eventName = ProcessEventName(eventName);

            // Check if there are any subscriptions for the event
            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {
                // No subscriptions found, no need to process further
                return false;
            }

            // Retrieve the list of subscriptions for the event
            var subscriptions = SubsManager.GetHandlersForEvent(eventName);

            using (var scope = ServiceProvider.CreateScope())
            {
                foreach (var subscription in subscriptions)
                {
                    // Get the handler for the subscription
                    var handler = ServiceProvider.GetService(subscription.HandlerType);

                    // If handler is not available, skip to the next subscription
                    if (handler == null)
                    {
                        continue;
                    }

                    // Get the event type based on the processed event name
                    var eventType = SubsManager.GetEventTypeByName(
                        $"{EventBusConfig.EventNamePrefix}{eventName}{EventBusConfig.EventNameSuffix}");

                    // Deserialize the message to the corresponding event type
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                    // Create the concrete handler type using the event type
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    // Get the Handle method of the handler
                    var handleMethod = concreteType.GetMethod("Handle");

                    // Invoke the Handle method asynchronously with the integration event
                    await (Task)handleMethod.Invoke(handler, new object[] { integrationEvent });
                }
            }

            // All subscriptions have been processed
            return true;
        }


        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;

        public abstract void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}