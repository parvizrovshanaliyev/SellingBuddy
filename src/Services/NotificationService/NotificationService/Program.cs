using System;
using System.Reflection;
using EventBus.Base;
using EventBus.Factory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Order;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace NotificationService
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World! Notification service");

            var serviceCollection = new ServiceCollection();
            
            // Load configuration from JSON and environment variables
            var configuration = ConfigureAppSettings();

            ConfigureService(serviceCollection, configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            IEventBus eventBus = serviceProvider.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
            eventBus.Subscribe<OrderPaymentSuccessIntegrationEvent, OrderPaymentSuccessIntegrationEventHandler>();

            Console.WriteLine("Application is Running ...");

            Console.ReadLine();
        }

        // Method to load appsettings.json and environment variables
        private static IConfiguration ConfigureAppSettings()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load default settings
                .AddJsonFile("SharedAppSettings.json", optional: false, reloadOnChange: true) // Load default settings
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true) // Load environment-specific settings
                .AddJsonFile($"SharedAppSettings.{environment}.json", optional: true, reloadOnChange: true) // Load environment-specific settings
                .AddEnvironmentVariables(); // Add environment variables

            return builder.Build();
        }

        private static void ConfigureService(IServiceCollection services, IConfiguration configuration)
        {
            // Add configuration to services
            services.AddSingleton(configuration);

            // Configure logging
            services.AddLogging(configure =>
            {
                configure.AddConsole();
                configure.AddDebug();
                configure.AddConfiguration(configuration.GetSection("Logging"));
            });

            // Add integration event handlers
            services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
            services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

            // Add RabbitMQ event bus configuration
            services.AddSingleton(sp =>
                EventBusFactory.Create(
                    EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name, configuration),
                    sp));
        }
    }
}
