using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NotificationService.IntegrationEvents.Order;

public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
    {
        OrderId = orderId;
        ErrorMessage = errorMessage;
    }

    public int OrderId { get; }
    public string ErrorMessage { get; }
}

/// <summary>
/// 
/// </summary>
public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    private readonly ILogger<OrderPaymentFailedIntegrationEventHandler> _logger;

    public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEventHandler> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="event"></param>
    /// <returns></returns>
    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        // Send Fail Notification (Sms, Email, Push)
        _logger.LogInformation($"Order payment failed with OrderId:  { @event.OrderId}, ErrorMessage: {@event.ErrorMessage}");
        
        return Task.CompletedTask;
    }
}