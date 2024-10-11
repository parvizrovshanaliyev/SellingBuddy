using System;
using System.Linq;
using System.Net;
using Api.Shared.Consul;
using IdentityService.Api.Application.Services;
using IdentityService.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace IdentityService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Get the environment-configured URLs (fallback to default if not set)
        var urls = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5001";
        var uri = new Uri(urls);
        var port = uri.Port;
        
        builder.WebHost.UseKestrel(options =>
        {
            // Get the server's local IP address
            var ip = Dns.GetHostAddresses(Dns.GetHostName())
                .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        
            if (ip == null)
            {
                throw new Exception("No IPv4 address found for the server.");
            }
        
            // Listen on the server's IP on the specified port for HTTP
            options.Listen(ip, port);
        
            urls = $"http://{ip}:{port}";
        
            // Also listen on any IP address on the specified port
            options.Listen(IPAddress.Any, port);
        });

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityService.Api", Version = "v1" });
        });
        builder.Services.AddScoped<IIdentityService, IdentityService.Api.Application.Services.IdentityService>();
        builder.Services.AddConsulClient(builder.Configuration);
        
        var app = builder.Build();
        
        var registeredUrl = urls.Split(';').First();
        // Register the service with Consul, providing a default URL in case none is configured
        app.UseConsulRegister(
            builder.Configuration,
            app.Lifetime,
            defaultUrl: registeredUrl); // Default URL if no address is configured

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityService.Api v1"));
        }

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.Run();
    }
}