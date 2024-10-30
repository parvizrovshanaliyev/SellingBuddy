using System.Reflection;
using Api.Shared.Extensions;
using EventBus.Base;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PaymentService.Api.Extensions;
using PaymentService.Api.IntegrationEvents.Order;

namespace PaymentService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.AddSharedAppSettingsAndEnvironmentVariables(args);
            
            builder.Services.AddServices(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);
        
            builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();
        
            builder.Services.AddSingleton(sp =>
                EventBusFactory.Create(
                    EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name, builder.Configuration),
                    sp));
        }
        
        
        var app = builder.Build();
        {
            app.UseServices(builder.Configuration, app.Lifetime, app.Environment);
            app.RegisterEvents();
            app.Run();
        }
    }
}