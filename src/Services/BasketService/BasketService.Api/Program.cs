using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Api.Shared.Auth;
using Api.Shared.Caching;
using Api.Shared.Consul;
using Api.Shared.Host;
using BasketService.Api.Core.Infrastructure.Repository;
using BasketService.Api.Core.Infrastructure.Services;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BasketService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
                reloadOnChange: true)
            .AddJsonFile("SharedAppSettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"SharedAppSettings.{builder.Environment.EnvironmentName}.json", optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.WebHost.ConfiguredKestrel(builder.Configuration, out var urls);

        builder.Services.AddConsulClient(builder.Configuration);

        builder.Services.AddAuthService(builder.Configuration);

        builder.Services.AddCaching(builder.Configuration);
        
        builder.Services.AddSingleton(sp =>
            EventBusFactory.Create(
                EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name),
                sp));

        builder.Services.AddHttpContextAccessor();
        
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        
        builder.Services.AddScoped<IBasketRepository , BasketRepository>();

        // builder.Services.AddSingleton<IEventBus>(sp =>
        // {
        //     var correlationService = sp.GetRequiredService<ICorrelationService>();
        //
        //     EventBusConfig config = new()
        //     {
        //         ConnectionRetryCount = 5,
        //         EventNameSuffix = "IntegrationEvent",
        //         SubscriberClientAppName = "BasketService",
        //         Connection = new ConnectionFactory(),
        //         EventBusType = EventBusType.RabbitMQ,
        //     };
        //     
        //     return EventBusFactory.Create(config, sp, correlationService);
        // });

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BasketService.Api", Version = "v1" });
        });

        var app = builder.Build();

        var registeredUrl = urls.Split(';').First();
        app.UseConsulRegister(
            builder.Configuration,
            app.Lifetime,
            defaultUrl: registeredUrl);

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BasketService.Api v1"));
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}