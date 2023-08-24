using System.Threading.Tasks;
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction;

/// <summary>
///     markup interface
/// </summary>
public interface IntegrationEventHandler
{
}

public interface IIntegrationEventHandler<TIntegrationEvent> : IntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent @event);
}