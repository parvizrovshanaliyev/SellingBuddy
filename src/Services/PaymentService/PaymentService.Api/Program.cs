using System;
using System.IO;
using System.Reflection;
using EventBus.Base;
using EventBus.Factory;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PaymentService.Api.Extensions;
using PaymentService.Api.IntegrationEvents.Order;

public class Program
{
    /// <summary>
    ///   The main entry point for the application.
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true,
                    reloadOnChange: true)
                .AddJsonFile("SharedAppSettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"SharedAppSettings.{builder.Environment.EnvironmentName}.json", optional: true,
                    reloadOnChange: true)
                .AddEnvironmentVariables();
            
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
                        Email = "your.email@example.com"
                    }
                });
        
                // Include XML comments for Swagger documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        
            builder.Services.AddTransient<OrderStartedIntegrationEventHandler>();
        
            builder.Services.AddSingleton(sp =>
                EventBusFactory.Create(
                    EventBusConfig.GetRabbitMQConfig(Assembly.GetExecutingAssembly().GetName().Name, builder.Configuration),
                    sp));
        }
        
        
        var app = builder.Build();
        {
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment API v1"); });
            } 
            
            app.UseRouting();
        
            app.UseAuthorization();
            app.MapControllers();
            app.RegisterEvents();
            app.Run();
        }
    }
}