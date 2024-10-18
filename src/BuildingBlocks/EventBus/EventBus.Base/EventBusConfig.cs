using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

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

    public static EventBusConfig GetRabbitMQConfig(string subscriberClientAppName, IConfiguration configuration)
    {
        return new EventBusConfig
        {
            ConnectionRetryCount = 5,
            SubscriberClientAppName = subscriberClientAppName,
            DefaultTopicName = "SellingBuddyTopicName",
            EventBusType = EventBusType.RabbitMQ,
            EventNameSuffix = "IntegrationEvent",
            //EventBusConnectionString = eventBusConnectionString
            Connection = new ConnectionFactory()
            {
                HostName = configuration?["RabbitMQConfig:HostName"] ?? "localhost",
                Port = int.Parse(configuration?["RabbitMQConfig:Port"] ?? "5672"),
                UserName = configuration?["RabbitMQConfig:UserName"] ?? "guest",
                Password = configuration?["RabbitMQConfig:Password"] ?? "guest"
            }
        };
    }
}

public enum EventBusType
{
    RabbitMQ = 0,
    AzureServiceBus = 1
}