using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api.Shared.Consul;

public static class ConfigureServices
{
    public static IServiceCollection AddConsulClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["ConsulConfig:Address"];
            consulConfig.Address = new Uri(address);
        }));
        
        return services;
    }

    public static IApplicationBuilder UseConsulRegister(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IHostApplicationLifetime lifetime,
        string defaultUrl)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();
        
        // Retrieve the server addresses
        var serverAddresses = app.ServerFeatures.Get<IServerAddressesFeature>();

        if (serverAddresses == null || !serverAddresses.Addresses.Any())
        {
            if (string.IsNullOrWhiteSpace(defaultUrl))
            {
                throw new Exception("No server addresses were configured, and no default URL was provided.");
            }

            serverAddresses?.Addresses.Add(defaultUrl);
        }

        // Get the first address (you can adapt this logic as needed)
        var address = serverAddresses?.Addresses.FirstOrDefault() ?? defaultUrl;
        var uri = new Uri(address);
        
        // Register service with consul
        var registration = new AgentServiceRegistration()
        {
            ID = configuration["ConsulConfig:ServiceId"],
            Name = configuration["ConsulConfig:ServiceName"],
            Address = $"{uri.Host}",
            Port = uri.Port,
            Tags = new[] { configuration["ConsulConfig:ServiceId"] }
        };

        logger.LogInformation($"Registering service with consul {registration.Name} {registration.ID}");

        consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        consulClient.Agent.ServiceRegister(registration).Wait();

        lifetime.ApplicationStopping.Register(() =>
        {
            logger.LogInformation($"Deregistering service with consul {registration.Name} {registration.ID}");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
        });

        return app;
    }
}