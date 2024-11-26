using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static partial class ConfigureServices
{
    /// <summary>
    ///  Add CORS policy
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
    }
    
    /// <summary>
    ///   Use CORS policy
    /// </summary>
    /// <param name="app"></param>
    public static void UseCorsPolicy(this IApplicationBuilder app)
    {
        app.UseCors("CorsPolicy");
    }
}