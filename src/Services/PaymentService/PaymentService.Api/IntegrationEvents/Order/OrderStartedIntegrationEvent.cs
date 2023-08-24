using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PaymentService.Api.IntegrationEvents.Order;

public class OrderStartedIntegrationEvent : IntegrationEvent
{
    public OrderStartedIntegrationEvent()
    {
    }

    public OrderStartedIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }

    public int OrderId { get; set; }
}

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
            paymentSuccessFlag, @event.Id);

        // paymentEvent.CorrelationId = @event.CorrelationId;

        // Log.BindProperty("CorrelationId", @event.CorrelationId, false, out LogEventProperty p);

        _eventBus.Publish(paymentEvent);

        return Task.CompletedTask;
    }
}