using Api.Shared.Caching;
using Api.Shared.Serializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ConfigureServices
{
    public static IServiceCollection AddCaching(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddTransient<ISerializerService, NewtonSoftService>();
        
        var settings = configuration.GetSection(nameof(CacheConfig)).Get<CacheConfig>();
        if (settings == null) return services;
        if (settings.UseDistributedCache)
        {
            if (settings.PreferRedis)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = settings.RedisURL;
                    options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions()
                    {
                        AbortOnConnectFail = true,
                        EndPoints = { settings.RedisURL }
                    };
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }
        
            services.AddTransient<ICacheService, DistributedCacheService>();
        }
        else
        {
            services.AddTransient<ICacheService, LocalCacheService>();
        }
        
        // Register Redis ConnectionMultiplexer
        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(settings.RedisURL));
        
        services.AddScoped<IRedisService, RedisService>();
        services.AddMemoryCache();

        return services;
    }
}