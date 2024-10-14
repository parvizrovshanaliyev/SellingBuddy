using Api.Shared.Auth;
using Api.Shared.Consul;
using Api.Shared.Host;
using IdentityService.Api.Application.Services;
using Microsoft.OpenApi.Models;

namespace IdentityService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("SharedAppSettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"SharedAppSettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        
        builder.WebHost.ConfiguredKestrel(builder.Configuration, out var urls);
        
        builder.Services.AddConsulClient(builder.Configuration);
        
        builder.Services.AddJwtConfiguration(builder.Configuration);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityService.Api", Version = "v1" });
        });
        builder.Services.AddScoped<IIdentityService, IdentityService.Api.Application.Services.IdentityService>();
        
        
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