using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Utils;

namespace WebApp;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

        builder.Services.AddBlazoredLocalStorage();
        
        builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        
        builder.Services.AddScoped(serviceProvider =>
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            
            return httpClientFactory.CreateClient("ApiGatewayHttpClient");
        });

        builder.Services.AddHttpClient("ApiGatewayHttpClient",
            client => client.BaseAddress = new Uri("http://localhost:5000/"));

        await builder.Build().RunAsync();
    }
}