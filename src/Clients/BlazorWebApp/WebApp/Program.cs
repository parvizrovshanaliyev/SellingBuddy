using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Application.Services.Identity;
using WebApp.Utils;

namespace WebApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        // Root component
        builder.RootComponents.Add<App>("#app");

        // Default HttpClient setup (used for base address defined by the app environment)
        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        // Add Blazored Local Storage
        builder.Services.AddBlazoredLocalStorage();

        // Authentication and Identity services
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();

        // Register the default ApiGatewayHttpClient with a proper base address
        builder.Services.AddHttpClient("ApiGatewayHttpClient", client =>
        {
            // Consider using environment-specific URLs (dev, prod)
            var apiBaseAddress = builder.HostEnvironment.IsDevelopment() 
                ? "http://localhost:5000/"  // Local development URL
                : "https://your-production-url.com/";  // Production URL
            
            client.BaseAddress = new Uri(apiBaseAddress);
        });

        // HttpClient factory for dependency injection
        builder.Services.AddScoped(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            return httpClientFactory.CreateClient("ApiGatewayHttpClient");
        });

        await builder.Build().RunAsync();
    }
}
