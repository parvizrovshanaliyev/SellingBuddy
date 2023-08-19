using System;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;

namespace EventBus.UnitTest.Events.Order
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public new int Id { get; set; }

        public OrderCreatedIntegrationEvent(int id)
        {
            Id = id;
        }
    }

    public class OrderCreatedIntegrationEventHandlers : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            Console.WriteLine("Order created event worked eventId :{eventId}", @event.Id);
            return Task.CompletedTask;
        }
    }
}
