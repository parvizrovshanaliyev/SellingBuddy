using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Api.Shared.Auth;

public static class ConfigureServices
{
    public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection(nameof(JwtConfig)));
        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        return services;
    }
    
    public static IServiceCollection AddAuthService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection(nameof(JwtConfig)));
        
        var jwtConfig = new JwtConfig();
    
        configuration.GetSection(nameof(JwtConfig)).Bind(jwtConfig);
        
        var jwtConfigSecret = jwtConfig.Secret;
        
        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfigSecret));
        
        services.AddJwtConfiguration(configuration);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                };
            });
        
        return services;
    }
}