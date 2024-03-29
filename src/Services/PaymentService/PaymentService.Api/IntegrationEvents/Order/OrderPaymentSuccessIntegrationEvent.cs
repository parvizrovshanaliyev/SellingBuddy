using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PaymentService.Api.IntegrationEvents.Order;

/// <summary>
/// 
/// </summary>
public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderId"></param>
    public OrderPaymentSuccessIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }

    /// <summary>
    /// 
    /// </summary>
    public int OrderId { get; }
}

/// <summary>
/// 
/// </summary>
public class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
{
    private readonly ILogger<OrderPaymentSuccessIntegrationEventHandler> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEventHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        _logger.LogInformation("OrderPaymentSuccessIntegration event received " + @event.OrderId);
        
        return Task.CompletedTask;
    }
}