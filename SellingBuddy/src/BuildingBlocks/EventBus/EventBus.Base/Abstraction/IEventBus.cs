﻿using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    /// <summary>
    /// subscription
    /// servicelerimiz subscribe olacaq
    /// </summary>
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);
        void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        void UnSubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
    }
}
