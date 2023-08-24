using System;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;

namespace PaymentService.Api.IntegrationEvents.Order;

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
        _logger.LogInformation("OrderPaymentFailedIntegration event received  " + @event.OrderId);
        
        return Task.CompletedTask;
    }
}