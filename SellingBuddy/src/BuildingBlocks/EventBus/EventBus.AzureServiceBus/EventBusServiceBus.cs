using System;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus: BaseEventBus
    {
        #region fields

        private ITopicClient _topicClient;
        private readonly ManagementClient _managementClient;

        #endregion
        #region ctor
        public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            _managementClient = new ManagementClient(config.EventBusConnectionString);
            _topicClient = CreateTopicClient();
        }
        #endregion


        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;// example OrderCreatedIntegrationEvent
            eventName = ProcessEventName(eventName); // example : OrderCreated

            var eventStr=JsonConvert.SerializeObject(@event);
            var bodyArr = Encoding.UTF8.GetBytes(eventStr);
            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = bodyArr,
                Label = eventName

            };
            _topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {
            var eventName =typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {

            }
        }

        public override void UnSubscribe<T, TH>()
        {
            throw new NotImplementedException();
        }

        #region private methods

        private ITopicClient CreateTopicClient()
        {
            if (_topicClient is null || _topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(EventBusConfig.EventBusConnectionString,
                    EventBusConfig.DefaultTopicName,RetryPolicy.Default);
            }

            // ensure that already topic exist
            if (!_managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
            {
                _managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();
            }

            return _topicClient;
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName,
                GetSubName(eventName));
        }

        //private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
        //{
        //    var subClient = CreateSubscriptionClient(eventName);

        //    _
        //}
        #endregion
    }
}
