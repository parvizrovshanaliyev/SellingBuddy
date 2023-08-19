using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.Order;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace EventBus.UnitTest
{
    [TestClass]
    public class EventBusTests
    {
        private ServiceCollection _services;

        public EventBusTests()
        {
            _services = new ServiceCollection();

            _services.AddLogging(configure => configure.AddConsole());
        }


        [TestMethod]
        public void subscribe_event_on_rabbitmq_test()
        {
            _services.AddSingleton<IEventBus>(sp => EventBusFactory.Create(GetRabbitMQConfig(),sp));

            var sp = _services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();
            
            
            eventBus.Subscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandlers>();
            eventBus.UnSubscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandlers>();
        }

        [TestMethod]
        public void send_message_to_rabbitmq_test()
        {
            _services.AddSingleton<IEventBus>(sp => EventBusFactory. Create(GetRabbitMQConfig(), sp));
            
            var sp = _services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();
            
            eventBus.Publish(new OrderCreatedIntegrationEvent(1));
        }
        
        [TestMethod]
        public void consume_order_created_from_rabbitmq_test()
        {
            _services.AddSingleton<IEventBus>(sp => EventBusFactory. Create(GetRabbitMQConfig(), sp));
            
            var sp = _services.BuildServiceProvider();
            var eventBus = sp.GetRequiredService<IEventBus>();
            
            eventBus.Subscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandlers>();
        }

        private static EventBusConfig GetRabbitMQConfig()
        {
            return new EventBusConfig()
            {
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "SellingBuddyTopicName",
                EventBusType = EventBusType.RabbitMQ,
                EventNameSuffix = "IntegrationEvent",
                EventBusConnectionString = "amqps://wtjzmmla:1GNs9JSK1kfinUeiyyahyyay3URIUxaS@toad.rmq.cloudamqp.com/wtjzmmla"
                //Connection = new ConnectionFactory ()
                //{
                    // HostName = "localhost",
                    // Port = 15672,
                    // //11 UserName = "guest",
                    // Password = "guest"
                //}
            };
        }
    }
}