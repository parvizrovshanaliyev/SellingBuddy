using System;
using System.Net.Http;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using WebApp.Application.Services.Identity;
using WebApp.Infrastructure.Identity;
using WebApp.Infrastructure.Indentity;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Blazored Local Storage
        services.AddBlazoredLocalStorage();

        // Authentication and Identity services
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        services.AddScoped<IIdentityService, IdentityService>();

        // Register the default ApiGatewayHttpClient with a proper base address
        services.AddHttpClient("ApiGatewayHttpClient", client =>
        {
            // Consider using environment-specific URLs (dev, prod)
            var apiBaseAddress = "http://localhost:5000/";  // Local API Gateway
            
            client.BaseAddress = new Uri(apiBaseAddress);
        });

        // HttpClient factory for dependency injection
        services.AddScoped(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            return httpClientFactory.CreateClient("ApiGatewayHttpClient");
        });
        
        return services;
    }
}