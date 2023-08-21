using System;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;

namespace PaymentService.Api.IntegrationEvents.Order;

public class OrderPaymentFailedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get;}
    public string ErrorMessage { get; }

    public OrderPaymentFailedIntegrationEvent(int orderId, string errorMessage)
    {
        OrderId = orderId;
        ErrorMessage = errorMessage;
    }
}


public class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    public Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        throw new NotImplementedException();
    }
}