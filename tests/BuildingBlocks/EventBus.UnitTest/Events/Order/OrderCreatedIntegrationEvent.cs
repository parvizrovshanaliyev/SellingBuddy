using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace EventBus.UnitTest.Events.Order;

public class OrderCreatedIntegrationEvent : IntegrationEvent
{

}

public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
{
    public Task Handle(OrderCreatedIntegrationEvent @event)
    {
        Logger.LogMessage("OrderCreatedIntegrationEvent handler called with event: " + @event.Id);
        
        return Task.CompletedTask;
    }
}