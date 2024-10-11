using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace CatalogService.Api.Extensions;

public static class ConsulRegistration
{
    public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["ConsulConfig:Address"];
            consulConfig.Address = new Uri(address);
        }));
        
        return services;
    }

    public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IConfiguration configuration,
        IHostApplicationLifetime lifetime)
    {
        var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

        //Get server IP address
        var features = app.Properties["server.Features"] as FeatureCollection;
        var addresses = features.Get<IServerAddressesFeature>();
        var address = addresses.Addresses.First();

        // Register service with consul
        var uri = new Uri(address);
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