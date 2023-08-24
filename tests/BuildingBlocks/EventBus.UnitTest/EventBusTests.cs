using System;
using System.Threading.Tasks;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.Order;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventBus.UnitTest;

[TestClass]
public class EventBusTests
{
    private readonly ServiceCollection _services;

    public EventBusTests()
    {
        _services = new ServiceCollection();

        _services.AddLogging(configure => configure.AddConsole());
    }


    [TestMethod]
    public void subscribe_event_on_rabbitmq_test()
    {
        _services.AddSingleton(sp => EventBusFactory.Create(GetRabbitMQConfig(), sp));
        _services.AddTransient<OrderCreatedIntegrationEventHandler>();

        var sp = _services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent,OrderCreatedIntegrationEventHandler>();

        Task.Delay(2000).Wait();
    }

    [TestMethod]
    public void send_message_to_rabbitmq_test()
    {
        _services.AddSingleton(sp => EventBusFactory.Create(GetRabbitMQConfig(), sp));

        var sp = _services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent());
    }

    [TestMethod]
    public void consume_order_created_from_rabbitmq_test()
    {
        _services.AddSingleton(sp => EventBusFactory.Create(GetRabbitMQConfig(), sp));
        // Register event handlers
        _services.AddTransient<OrderCreatedIntegrationEventHandler>(); // Add other handlers as needed
        var sp = _services.BuildServiceProvider();
        var eventBus = sp.GetRequiredService<IEventBus>();
        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
    }

    [TestMethod]
    public void DeserializeEvent_ValidJson_ReturnsIntegrationEvent()
    {
        _services.AddSingleton(sp => EventBusFactory.Create(GetRabbitMQConfig(), sp));

        var sp = _services.BuildServiceProvider();
        var eventBus = sp.GetRequiredService<IEventBus>();
        var eventJson = "{\"Id\":1,\"CreatedDate\":\"2023-08-24T15:50:22.6561522+04:00\"}";
        var eventType = typeof(OrderCreatedIntegrationEvent);


        var deserializedEvent = eventBus.DeserializeEvent(eventJson, eventType);

        Console.WriteLine(deserializedEvent);

        // Assert
        Assert.IsNotNull(deserializedEvent);
        Assert.IsInstanceOfType(deserializedEvent, eventType);
        var integrationEvent = (OrderCreatedIntegrationEvent)deserializedEvent;
        //Assert.AreEqual(1, integrationEvent.Id);
        Assert.AreEqual(new DateTime(2023, 8, 24, 15, 50, 22, DateTimeKind.Utc), integrationEvent.CreatedDate);
    }

    private static EventBusConfig GetRabbitMQConfig()
    {
        return new EventBusConfig
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = "EventBus.UnitTest",
            DefaultTopicName = "SellingBuddyTopicName",
            EventBusType = EventBusType.RabbitMQ,
            EventNameSuffix = "IntegrationEvent",
            EventBusConnectionString =
                "amqps://wtjzmmla:1GNs9JSK1kfinUeiyyahyyay3URIUxaS@toad.rmq.cloudamqp.com/wtjzmmla"
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