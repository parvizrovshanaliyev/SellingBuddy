using System;
using System.Net.Http;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using WebApp.Application.Services.Identity;
using WebApp.Infrastructure;
using WebApp.Infrastructure.HttpClient;
using WebApp.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AppStateManager>();
        
        // Add Blazored Local Storage services for client-side storage management
        services.AddBlazoredLocalStorage();

        // Register custom authentication state provider and identity service for authentication handling
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        services.AddScoped<IIdentityService, IdentityService>();

        // Register the AuthTokenHandler and ISyncLocalStorageService
        services.AddScoped<AuthTokenHandler>();

        // Configure an HTTP client for the API Gateway, including the AuthTokenHandler in the pipeline
        services.AddHttpClient("ApiGatewayHttpClient", client =>
            {
                // Retrieve the API Gateway base address from configuration (supports environment-specific values)
                var apiBaseAddress = configuration["ApiSettings:BaseAddress"] ?? "http://localhost:5000/";  // Default to localhost if not configured
            
                client.BaseAddress = new Uri(apiBaseAddress);
                client.DefaultRequestHeaders.Add("Accept", "application/json");  // Optional: Set default headers like Accept
            })
            .AddHttpMessageHandler<AuthTokenHandler>(); // Add the AuthTokenHandler to the pipeline

        // Create a scoped HTTP client for dependency injection
        services.AddScoped(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            return httpClientFactory.CreateClient("ApiGatewayHttpClient");
        });

        return services;
    }
}