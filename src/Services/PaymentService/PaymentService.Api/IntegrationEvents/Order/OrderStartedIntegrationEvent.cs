using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PaymentService.Api.IntegrationEvents.Order;

/// <summary>
/// 
/// </summary>
public class OrderStartedIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderId"></param>
    [JsonConstructor]
    public OrderStartedIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }

    /// <summary>
    /// 
    /// </summary>
    public int OrderId { get; set; }
}

/// <summary>
/// 
/// </summary>
public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderStartedIntegrationEventHandler> _logger;

    /// <summary>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="eventBus"></param>
    /// <param name="logger"></param>
    public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus,
        ILogger<OrderStartedIntegrationEventHandler> logger)
    {
        _configuration = configuration;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public Task Handle(OrderStartedIntegrationEvent @event)
    {
        // fake payment process
        var keyword = "PaymentSuccess";
        var paymentSuccessFlag = _configuration.GetValue<bool>(keyword);

        IntegrationEvent paymentEvent = paymentSuccessFlag
            ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
            : new OrderPaymentFailedIntegrationEvent(@event.OrderId, "This is a fake error message");

        _logger.LogInformation(
            "OrderCreatedIntegrationEventHandler in PaymentService is fired with PaymentSuccess : {flag}, orderId : {orderId}",
            paymentSuccessFlag, @event.OrderId);

        // paymentEvent.CorrelationId = @event.CorrelationId;

        // Log.BindProperty("CorrelationId", @event.CorrelationId, false, out LogEventProperty p);

        _eventBus.Publish(paymentEvent);

        return Task.CompletedTask;
    }
}