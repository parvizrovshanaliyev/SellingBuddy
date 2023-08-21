using System;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;

namespace PaymentService.Api.IntegrationEvents.Order;

public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    public int OrderId { get;}

    public OrderPaymentSuccessIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }
}

public class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
{
    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        throw new NotImplementedException();
    }
}