using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NotificationService.IntegrationEvents.Order;

/// <summary>
/// 
/// </summary>
public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    public OrderPaymentSuccessIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }

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

    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        // Send Fail Notification (Sms, Email, Push)
        
        _logger.LogInformation($"Order payment success with OrderId:  { @event.OrderId}");
        
        return Task.CompletedTask;
    }
}