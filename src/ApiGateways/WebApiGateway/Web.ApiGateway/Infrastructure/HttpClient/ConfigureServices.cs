namespace Web.ApiGateway.Infrastructure.HttpClient;

/// <summary>
/// This static class contains methods for adding and configuring HttpClient services 
/// in the dependency injection container, including custom message handlers 
/// and configuration-based settings for each client.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    /// Adds custom HttpClient services to the dependency injection container.
    /// Configures HttpClients for services like "basket" and "catalog" with 
    /// base addresses and optional timeouts from configuration.
    /// Also registers the HttpContextAccessor and the custom delegating handler 
    /// for adding authorization headers and logging.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The application's IConfiguration instance for retrieving configuration values.</param>
    /// <returns>The updated IServiceCollection with added HttpClients and handlers.</returns>
    public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        // Register HttpContextAccessor for accessing HTTP context in delegating handler
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // Register custom delegating handler for Authorization and logging
        services.AddTransient<HttpClientDelegatingHandler>();

        // Add and configure the HttpClient for Basket service using configuration values
        ConfigureHttpClient(services, configuration, "HttpClient:Basket", "basket");

        // Add and configure the HttpClient for Catalog service using configuration values
        ConfigureHttpClient(services, configuration, "HttpClient:Catalog", "catalog");

        return services;
    }

    /// <summary>
    /// Configures an individual HttpClient for a given service using base address 
    /// and optional timeout settings from the configuration.
    /// Adds a custom message handler to inject authorization headers and handle logging.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the HttpClient to.</param>
    /// <param name="configuration">The application's IConfiguration instance for retrieving configuration values.</param>
    /// <param name="configSection">The section of the configuration containing the settings for the HttpClient.</param>
    /// <param name="clientName">The name of the client (e.g., "basket" or "catalog").</param>
    private static void ConfigureHttpClient(IServiceCollection services, IConfiguration configuration, string configSection, string clientName)
    {
        // Retrieve base address from configuration
        var baseAddress = configuration[$"{configSection}:BaseAddress"];
        
        // Ensure that base address is configured
        if (string.IsNullOrEmpty(baseAddress))
        {
            throw new InvalidOperationException($"Base address for '{clientName}' is not configured.");
        }

        // Configure HttpClient for the specified service
        services.AddHttpClient(clientName, client =>
        {
            // Set the base address and common headers
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", $"{clientName}-HttpClient");

            // Optional: Set a timeout from configuration if available
            if (int.TryParse(configuration[$"{configSection}:TimeoutInSeconds"], out var timeoutInSeconds))
            {
                client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            }
        })
        .AddHttpMessageHandler<HttpClientDelegatingHandler>(); // Add custom message handler for authorization and logging
    }
}

