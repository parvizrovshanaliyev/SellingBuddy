using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        #region fields

        private readonly RabbitMQPersistentConnection _persistentConnection;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IModel _consumerChannel;
        private int _retryCount;

        #endregion

        #region ctors

        public EventBusRabbitMQ(
            IServiceProvider serviceProvider,
            EventBusConfig config) : base(serviceProvider,
            config)
        {
            if (config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(
                    EventBusConfig.Connection,
                    settings: new JsonSerializerSettings()
                    {
                        // Self referencing loop detected for property
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                _connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
            }
            else
            {
                _connectionFactory = new ConnectionFactory();
            }

            _persistentConnection = new RabbitMQPersistentConnection(
                connectionFactory: _connectionFactory,
                retryCount: config.ConnectionRetryCount);

            _consumerChannel = CreateConsumerChannel();

            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
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
                    onRetry:(ex, time) =>
                    {
                        //log
                    });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            _consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName,
                type: "direct"); // Ensure exchange exists while publishing

            var message = JsonConvert.SerializeObject(@event);
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
            eventName = ProcessEventName(eventName); // example : OrderCreated

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
            }

            SubsManager.AddSubscription<T, TH>();

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
            catch (Exception ex)
            {
                // log
            }

            _consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
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