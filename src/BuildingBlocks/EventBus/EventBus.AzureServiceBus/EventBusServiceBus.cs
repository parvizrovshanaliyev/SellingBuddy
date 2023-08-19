using System;
using System.Text;
using System.Threading.Tasks;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {
        #region fields
        private ITopicClient _topicClient;
        private ManagementClient _managementClient;
        private readonly ILogger _logger;
        #endregion

        #region ctor
        public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            _logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;
            _managementClient = new ManagementClient(config.EventBusConnectionString);
            _topicClient = CreateTopicClient();
        }
        #endregion


        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;// example OrderCreatedIntegrationEvent
            eventName = ProcessEventName(eventName); // example : OrderCreated

            var eventStr = JsonConvert.SerializeObject(@event);
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
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {
                var subscriptionClient = CreateSubscriptionClient(eventName: eventName);

                RegisterSubscriptionClientMessageHandler(subscriptionClient: subscriptionClient);
            }

            _logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            SubsManager.AddSubscription<T, TH>();
        }

        public override void UnSubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            try
            {
                // Subscription will be there but we don't subscribe
                var subscriptionClient = CreateSubscriptionClient(eventName);
                subscriptionClient
                    .RemoveRuleAsync(eventName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity (eventName} Could not be found.", eventName);
            }

            _logger.LogInformation("Unsubscribing from event {EventName}", eventName);
            SubsManager.RemoveSubscription<T, TH>();
        }

        public override void Dispose()
        {
            base.Dispose();

            
            _topicClient.CloseAsync().GetAwaiter().GetResult();
            _managementClient.CloseAsync();

            _topicClient = null;
            _managementClient = null;

        }

        #region private methods
        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);
                    // Complete the message so that it is not received again.
                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            _logger.LogError(
                ex,
                message: "ERROR handling message: {ExceptionMessage} - Context: {@ExceptionContext}", ex.Message,
                context);

            return Task.CompletedTask;
        }

        private ITopicClient CreateTopicClient()
        {
            if (_topicClient is null || _topicClient.IsClosedOrClosing)
            {
                _topicClient = new TopicClient(EventBusConfig.EventBusConnectionString,
                    EventBusConfig.DefaultTopicName, RetryPolicy.Default);
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



        private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);

            var exist = _managementClient.SubscriptionExistsAsync(
                topicPath: EventBusConfig.DefaultTopicName,
                subscriptionName: GetSubName(eventName)
            ).GetAwaiter().GetResult();

            if (!exist)
            {
                _managementClient.CreateSubscriptionAsync(
                    topicPath: EventBusConfig.DefaultTopicName,
                    subscriptionName: GetSubName(eventName))
                    .GetAwaiter().GetResult();

                RemoveDefaultRule(subClient);
            }

            CreateRuleIfNotExists(
                eventName: ProcessEventName(eventName),
                subscriptionClient: subClient);

            return subClient;

        }

        private void CreateRuleIfNotExists(string eventName, ISubscriptionClient subscriptionClient)
        {
            bool ruleExits;
            try
            {
                var rule = _managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName), eventName)
                    .GetAwaiter().GetResult();
                ruleExits = rule != null;
            }
            catch (MessagingEntityNotFoundException)
            {
                // Azure Management Client doesn't have RuleExists method
                ruleExits = false;
            }

            if (!ruleExits)
            {
                subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                }).GetAwaiter().GetResult();
            }
        }


        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                _logger.LogWarning("The messaging entity {DefaultRuleName} Could not be found.",
                    RuleDescription.DefaultRuleName);
            }
        }
        #endregion
    }
}
