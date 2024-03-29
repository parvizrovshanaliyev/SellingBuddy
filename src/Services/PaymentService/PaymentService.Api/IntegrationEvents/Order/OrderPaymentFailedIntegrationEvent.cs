using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PaymentService.Api.IntegrationEvents.Order;

/// <summary>
/// 
/// </summary>
public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="errorMessage"></param>
    public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
    {
        OrderId = orderId;
        ErrorMessage = errorMessage;
    }

    public int OrderId { get; set; }
    public string ErrorMessage { get; set; }
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
        _logger.LogInformation("OrderPaymentFailedIntegration event received  " + @event.OrderId);
        
        return Task.CompletedTask;
    }
}