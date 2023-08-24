using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;

namespace EventBus.UnitTest.Events.Order
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        public OrderCreatedIntegrationEvent(int id)
        {
            Id = id;
        }
    }

    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            Debug.WriteLine("Order created event worked eventId :{eventId}", @event.Id);
            return Task.CompletedTask;
        }
    }
}
