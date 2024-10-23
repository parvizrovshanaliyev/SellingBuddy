using Api.Shared.Configuration;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Policy = Polly.Policy;

namespace Api.Shared.Consul;

public static class ConfigureServices
{
    // Add Consul client to the service collection
    public static IServiceCollection AddConsulClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulConfig =>
        {
            var address = configuration["ConsulConfig:Address"];
            consulConfig.Address = new Uri(address);
        }));

        return services;
    }

    // Use Consul for registering services
    
     public static IApplicationBuilder UseConsulRegister(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IHostApplicationLifetime lifetime)
    {
        var consulClient = app.ApplicationServices.GetService<IConsulClient>();
        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();
        
        if (!(app.Properties["server.Features"] is FeatureCollection features)) return app;
 
        var addresses = features.Get<IServerAddressesFeature>();
        // Check if Consul is available
        if (!configuration.IsConsulAvailable())
        {
            logger.LogWarning("Consul is not available. Skipping Consul registration.");
            return app;
        }

        var defaultUrl = configuration.GetAspNetCoreUrls()?.Split(';').FirstOrDefault();

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

        // Polly retry policy: Retry 3 times with exponential backoff
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(3, retryAttempt =>
            {
                var waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)); // Exponential backoff
                logger.LogWarning($"Retrying Consul registration in {waitTime.TotalSeconds} seconds...");
                return waitTime;
            });

        try
        {
            // Execute Polly policy around Consul registration
            retryPolicy.Execute(() =>
            {
                logger.LogInformation($"Registering service with Consul {registration.Name} {registration.ID}");

                // Deregister any previous instance
                consulClient?.Agent.ServiceDeregister(registration.ID).Wait();

                // Register the current service instance
                consulClient?.Agent.ServiceRegister(registration).Wait();
            });

            // Register to deregister the service on application stop
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation($"Deregistering service with Consul {registration.Name} {registration.ID}");
                retryPolicy.Execute(() => consulClient?.Agent.ServiceDeregister(registration.ID).Wait());
            });
        }
        catch (Exception ex)
        {
            logger.LogError($"Error registering service with Consul: {ex.Message}");
            logger.LogWarning("Skipping Consul registration since Consul may not be running.");
        }

        return app;
    }
    
    public static IApplicationBuilder UseConsulRegister(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IHostApplicationLifetime lifetime,
        string defaultUrl)
    {
        var consulClient = app.ApplicationServices.GetService<IConsulClient>();
        var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
        var logger = loggingFactory.CreateLogger<IApplicationBuilder>();
        var lifetime2 = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
        
        if (!(app.Properties["server.Features"] is FeatureCollection features)) return app;
 
        var addresses = features.Get<IServerAddressesFeature>();
        var addresss = addresses.Addresses.FirstOrDefault();

        // Check if Consul is available
        if (!configuration.IsConsulAvailable())
        {
            logger.LogWarning("Consul is not available. Skipping Consul registration.");
            return app;
        }

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

        // Polly retry policy: Retry 3 times with exponential backoff
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetry(3, retryAttempt =>
            {
                var waitTime = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)); // Exponential backoff
                logger.LogWarning($"Retrying Consul registration in {waitTime.TotalSeconds} seconds...");
                return waitTime;
            });

        try
        {
            // Execute Polly policy around Consul registration
            retryPolicy.Execute(() =>
            {
                logger.LogInformation($"Registering service with Consul {registration.Name} {registration.ID}");

                // Deregister any previous instance
                consulClient?.Agent.ServiceDeregister(registration.ID).Wait();

                // Register the current service instance
                consulClient?.Agent.ServiceRegister(registration).Wait();
            });

            // Register to deregister the service on application stop
            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation($"Deregistering service with Consul {registration.Name} {registration.ID}");
                retryPolicy.Execute(() => consulClient?.Agent.ServiceDeregister(registration.ID).Wait());
            });
        }
        catch (Exception ex)
        {
            logger.LogError($"Error registering service with Consul: {ex.Message}");
            logger.LogWarning("Skipping Consul registration since Consul may not be running.");
        }

        return app;
    }

    public static bool IsConsulAvailable(this IConfiguration configuration)
    {
        var consulUrl = configuration["ConsulConfig:Address"] ?? "http://localhost:8500";
        using var httpClient = new HttpClient();

        // Polly retry policy for checking Consul availability
        var retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine(
                        $"Retry {retryCount} for Consul availability check. Waiting {timeSpan.TotalSeconds} seconds before next attempt.");
                });

        try
        {
            // Execute the policy, retrying up to 3 times
            var response = retryPolicy.Execute(() =>
            {
                Console.WriteLine("Checking Consul availability...");
                return httpClient.GetAsync($"{consulUrl}/v1/status/leader").Result;
            });

            // Check if the request was successful
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking Consul availability: {ex.Message}");
            return false;
        }
    }
}