using Api.Shared.Auth;
using Api.Shared.Consul;
using Api.Shared.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Api.Shared.Extensions;

public static class AppInitializerExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, string assemblyName)
    {
        services.AddControllers();
        
        services.AddEndpointsApiExplorer();
        
        services.AddSwaggerDocumentation(configuration, assemblyName);
        
        services.AddConsulClient(configuration);
        
        services.AddLogging(configure => configure.AddConsole());
        
        services.AddAuthService(configuration);
        
        services.AddHttpContextAccessor();

        return services;
    }
    
    public static IApplicationBuilder UseServices(
        this IApplicationBuilder app,
        IConfiguration configuration,
        IHostApplicationLifetime lifetime,
        IWebHostEnvironment environment)
    {
        app.UseConsulRegister(configuration, lifetime); 
        
        // Configure the HTTP request pipeline.
        if (environment.IsDevelopment())
        {
            app.UseSwaggerDocumentation(configuration);
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        return app;
    }
}