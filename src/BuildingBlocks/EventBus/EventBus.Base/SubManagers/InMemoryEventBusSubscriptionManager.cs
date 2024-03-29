﻿using System;
using System.Collections.Generic;
using System.Linq;
using EventBus.Base.Abstraction;
using EventBus.Base.Events;

namespace EventBus.Base.SubManagers;

public class InMemoryEventBusSubscriptionManager : IEventBusSubscriptionManager
{
    private readonly List<Type> _eventTypes;

    /// <summary>
    ///     handlerlerimizi saxlayacaq
    /// </summary>
    private readonly Dictionary<string, List<SubscriptionInfo>> _handlers;

    /// <summary>
    /// </summary>
    public Func<string, string> EventNameGetter;

    public InMemoryEventBusSubscriptionManager(Func<string, string> eventNameGetter)
    {
        _handlers = new Dictionary<string, List<SubscriptionInfo>>();
        _eventTypes = new List<Type>();
        EventNameGetter = eventNameGetter;
    }

    /// <summary>
    /// </summary>
    public bool IsEmpty => !_handlers.Keys.Any();

    /// <summary>
    /// </summary>
    public void Clear()
    {
        _handlers.Clear();
    }

    /// <summary>
    /// </summary>
    public event EventHandler<string> OnEventRemoved;

    public void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();


        AddSubscription(typeof(TH), eventName);

        if (!_eventTypes.Contains(typeof(T))) _eventTypes.Add(typeof(T));
    }

    public void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
    {
        var handlerToRemove = FindSubscriptionToRemove<T, TH>();
        var eventName = GetEventKey<T>();
        RemoveHandler(eventName, handlerToRemove);
    }

    public bool HasSubscriptionForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return HasSubscriptionForEvent(key);
    }

    public bool HasSubscriptionForEvent(string eventName)
    {
        return _handlers.ContainsKey(eventName);
    }

    public Type GetEventTypeByName(string eventName)
    {
        return _eventTypes.SingleOrDefault(t => t.Name == eventName);
    }


    public IEnumerable<SubscriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent
    {
        var key = GetEventKey<T>();
        return GetHandlersForEvent(key);
    }

    public IEnumerable<SubscriptionInfo> GetHandlersForEvent(string eventName)
    {
        return _handlers[eventName];
    }

    public string GetEventKey<T>()
    {
        var eventName = typeof(T).Name;
        return EventNameGetter(eventName);
    }

    private void AddSubscription(Type handlerType, string eventName)
    {
        if (!HasSubscriptionForEvent(eventName)) _handlers.Add(eventName, new List<SubscriptionInfo>());

        if (_handlers[eventName].Any(s => s.HandlerType == handlerType))
            throw new ArgumentException($"handler type {handlerType.Name} already registered for '{eventName}'",
                nameof(handlerType));

        _handlers[eventName].Add(SubscriptionInfo.Typed(handlerType));
    }

    private void RemoveHandler(string eventName, SubscriptionInfo subsToRemove)
    {
        if (subsToRemove != null)
        {
            _handlers[eventName].Remove(subsToRemove);
            if (!_handlers[eventName].Any())
            {
                _handlers.Remove(eventName);
                var eventType = _eventTypes.SingleOrDefault(e => e.Name == eventName);
                if (eventType != null) _eventTypes.Remove(eventType);

                RaiseOnEventRemoved(eventName);
            }
        }
    }


    #region helper methods

    private SubscriptionInfo FindSubscriptionToRemove<T, TH>() where T : IntegrationEvent
        where TH : IIntegrationEventHandler<T>
    {
        var eventName = GetEventKey<T>();
        return FindSubscriptionToRemove(eventName, typeof(TH));
    }

    private SubscriptionInfo FindSubscriptionToRemove(string eventName, Type handlerType)
    {
        if (!HasSubscriptionForEvent(eventName)) return null;

        return _handlers[eventName].SingleOrDefault(s => s.HandlerType == handlerType);
    }

    private void RaiseOnEventRemoved(string eventName)
    {
        var handler = OnEventRemoved;
        handler?.Invoke(this, eventName);
    }

    #endregion
}