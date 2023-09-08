using System;
using System.Reflection;
using EventBus.Base;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Order;

namespace NotificationService;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello World! Notification service");
        
        ServiceCollection services = new ServiceCollection();

        ConfigureService(services);

        var serviceProvider = services.BuildServiceProvider();

        IEventBus eventBus = serviceProvider.GetRequiredService<IEventBus>();
        
        eventBus.Subscribe<OrderPaymentFailedIntegrationEvent,OrderPaymentFailedIntegrationEventHandler>();
        eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent,OrderPaymentSuccessIntegrationEventHandler>();
        
        Console.WriteLine("Application is Running ...");

        Console.ReadLine();
    }

    private static void ConfigureService(ServiceCollection services)
    {
        services.AddLogging(configure =>
        {
            configure.AddConsole();
            configure.AddDebug();
        });
        
        services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
        services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

        services.AddSingleton(sp =>
            EventBusFactory.Create(
                EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name),
                sp));
    }
    
}