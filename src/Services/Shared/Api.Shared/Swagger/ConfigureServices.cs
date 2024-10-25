using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Api.Shared.Swagger;
public static class ConfigureServices
{
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration,string assemblyName)
    {
        var openApiInfo = new OpenApiInfo
        {
            Title = configuration["Swagger:Title"] ?? "API",
            Version = configuration["Swagger:Version"] ?? "v1"
        };
        
        var xmlFile = $"{assemblyName}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", openApiInfo);
            
            // Include XML comments for Swagger documentation
           
            c.IncludeXmlComments(xmlPath);

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description =
                    "JWT Authorization header using the Bearer scheme.Example: \"Authorization: Bearer {token}\""
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });

        return services;
    }
    
    // use SwaggerUI
    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, IConfiguration configuration)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", configuration["Swagger:Title"] ?? "API");
        });

        return app;
    }
}
