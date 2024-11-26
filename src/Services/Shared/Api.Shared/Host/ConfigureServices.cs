using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ConfigureServices
{
    public static IWebHostBuilder ConfiguredKestrel(this IWebHostBuilder builder, IConfiguration configuration, out string urls)
    {
        urls = null;
        
        builder.ConfigureKestrel((context, options) =>
        {
            // Get the environment-configured URLs (fallback to default if not set)
            var urls = configuration["ASPNETCORE_URLS"] ?? "http://localhost:5001";
            var uri = new Uri(urls);
            var port = uri.Port;

            // Get the server's local IP address (IPv4)
            var ip = Dns.GetHostAddresses(Dns.GetHostName())
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        
            if (ip == null)
            {
                throw new Exception("No IPv4 address found for the server.");
            }

            // Listen on the server's IP on the specified port for HTTP
            options.Listen(ip, port);
            urls = $"http://{ip}:{port}";

            // Also listen on any IP address (useful for external access)
            options.Listen(IPAddress.Any, port);

            // Optionally, you can configure HTTPS if needed
            // options.Listen(ip, port, listenOptions => 
            // {
            //     listenOptions.UseHttps();
            // });
        });
        
        return builder;
    }
}