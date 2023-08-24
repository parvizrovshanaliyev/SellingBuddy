using System;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;

namespace PaymentService.Api.IntegrationEvents.Order;

public class OrderPaymentSuccessIntegrationEvent : IntegrationEvent
{
    public OrderPaymentSuccessIntegrationEvent(int orderId)
    {
        OrderId = orderId;
    }

    public int OrderId { get; }
}

public class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
{
    public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
    {
        throw new NotImplementedException();
    }
}