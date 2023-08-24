using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        #region fields

        private readonly RabbitMQPersistentConnection _persistentConnection;
        private readonly IModel _consumerChannel;
        private readonly ILogger _logger;
        #endregion

        #region ctors

        public EventBusRabbitMQ(
            IServiceProvider serviceProvider,
            EventBusConfig config) : base(serviceProvider, config)
        {
            IConnectionFactory connectionFactory;

            if (config.Connection != null)
            {
                var connJson = JsonSerializer.Serialize(config.Connection, new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                });

                connectionFactory = JsonSerializer.Deserialize<ConnectionFactory>(connJson);
            }
            else
            {
                connectionFactory = new ConnectionFactory();
                connectionFactory.Uri = new Uri(config.EventBusConnectionString);
            }

            _persistentConnection = new RabbitMQPersistentConnection(
                connectionFactory: connectionFactory,
                retryCount: config.ConnectionRetryCount);

            _consumerChannel = CreateConsumerChannel();

            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;

            _logger = serviceProvider.GetService(typeof(ILogger<EventBusRabbitMQ>)) as ILogger<EventBusRabbitMQ>;
        }


        #endregion

        #region methods

        public override void Publish(IntegrationEvent @event)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(EventBusConfig.ConnectionRetryCount,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (ex, time, retryAttempt) =>
                    {
                        // Log the retry attempt and exception using a logger
                        _logger.LogError(ex, $"Retry attempt {retryAttempt}. Exception: {ex}");
                    });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            _consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                type: "direct"); // Ensure exchange exists while publishing

            // TODO: This should probably not be here
            // _consumerChannel.QueueBind(queue: GetSubName(eventName),
            //     exchange: EventBusConfig.DefaultTopicName,
            //     routingKey: eventName);

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = _consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                _consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exists while publishing
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _consumerChannel.BasicPublish(
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body
                );
            });
        }


        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name; // example OrderCreatedIntegrationEvent
            eventName = ProcessEventName(eventName); // example: OrderCreated

            // Log subscription attempt
            _logger.LogInformation($"Subscribing to event: {eventName}");

            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {
                if (!_persistentConnection.IsConnected)
                {
                    _persistentConnection.TryConnect();
                }

                _consumerChannel.QueueDeclare(queue: GetSubName(eventName), // Ensure queue exists while consuming
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                _consumerChannel.QueueBind(queue: GetSubName(eventName),
                    exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName);

                // Log queue creation and binding
                _logger.LogInformation($"Queue '{GetSubName(eventName)}' created and bound to exchange '{EventBusConfig.DefaultTopicName}'");
            }

            SubsManager.AddSubscription<T, TH>();

            // Log subscription added and start consuming
            _logger.LogInformation($"Subscription added for event: {eventName}");
            StartBasicConsume(eventName: eventName);
        }


        public override void UnSubscribe<T, TH>()
        {
            SubsManager.RemoveSubscription<T, TH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(
                exchange: EventBusConfig.DefaultTopicName,
                type: "direct");

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (_consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(_consumerChannel);
                consumer.Received += Consumer_Received;
                _consumerChannel.BasicConsume(queue: GetSubName(eventName),
                    autoAck: false,
                    consumer: consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            eventName = ProcessEventName(eventName);
            
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (ObjectDisposedException ex)
            {
                _logger.LogError(ex, "ObjectDisposedException occurred: {Message}", ex.Message);
                // Handle or log the exception appropriately
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "TaskCanceledException occurred: {Message}", ex.Message);
                // Handle or log the exception appropriately
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing event '{eventName}': {ex.Message}");
            }
            finally
            {
                _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            }
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            //using var channel = persistentConnection.CreateModel();
            _consumerChannel.QueueUnbind(
                queue: eventName,
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName);

            if (SubsManager.IsEmpty)
            {
                _consumerChannel.Close();
            }
        }

        #endregion
    }
}