using System;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Order;
using Polly;

namespace PaymentService.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder RegisterEvents(this IApplicationBuilder app)
    {
        
        IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<OrderStartedIntegrationEvent, OrderStartedIntegrationEventHandler>();
        
        return app;
    }
}