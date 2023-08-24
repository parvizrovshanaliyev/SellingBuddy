namespace EventBus.Base;

public class EventBusConfig
{
    public int ConnectionRetryCount { get; set; } = 5;
    public string DefaultTopicName { get; set; } = "SellingBuddyEventBus";
    public string EventBusConnectionString { get; set; } = string.Empty;
    public string SubscriberClientAppName { get; set; } = string.Empty;
    public string EventNamePrefix { get; set; } = string.Empty;
    public string EventNameSuffix { get; set; } = "IntegrationEvent";
    public EventBusType EventBusType { get; set; } = EventBusType.RabbitMQ;
    public object Connection { get; set; }

    public bool DeleteEventPrefix => !string.IsNullOrEmpty(EventNamePrefix);
    public bool DeleteEventSuffix => !string.IsNullOrEmpty(EventNameSuffix);

    public static EventBusConfig GetRabbitMQConfig(string subscriberClientAppName)
    {
        return new EventBusConfig
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = subscriberClientAppName,
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

public enum EventBusType
{
    RabbitMQ = 0,
    AzureServiceBus = 1
}