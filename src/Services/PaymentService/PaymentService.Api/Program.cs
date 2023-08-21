using System;
using System.IO;
using System.Reflection;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PaymentService.Api.Extensions;
using PaymentService.Api.IntegrationEvents.Order;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddLogging(configure =>
    {
        configure.AddConsole();
        configure.AddDebug();
    });

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Payment API",
            Version = "v1",
            Description = "API for managing payments.",
            Contact = new OpenApiContact
            {
                Name = "Your Name",
                Email = "your.email@example.com",
            }
        });

        // Include XML comments for Swagger documentation
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();

    builder.Services.AddSingleton<IEventBus>(sp => 
        EventBusFactory.Create(
            config:EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name),
            serviceProvider:sp));
}


var app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1"); });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.RegisterEvents();
    app.Run();
}