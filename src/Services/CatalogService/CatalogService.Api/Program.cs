using System.Net;
using System.Reflection;
using Api.Shared.Consul;
using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Get the environment-configured URLs (fallback to default if not set)
var urls = builder.Configuration["ASPNETCORE_URLS"] ?? "http://localhost:5001";
var uri = new Uri(urls);
var port = uri.Port;

builder.WebHost.UseKestrel(options =>
{
    // Get the server's local IP address
    var ip = Dns.GetHostAddresses(Dns.GetHostName())
        .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

    if (ip == null)
    {
        throw new Exception("No IPv4 address found for the server.");
    }

    // Listen on the server's IP on the specified port for HTTP
    options.Listen(ip, port);
    
    urls = $"http://{ip}:{port}";

    // Also listen on any IP address on the specified port
    options.Listen(IPAddress.Any, port);
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddLogging(configure => configure.AddConsole());

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Catalog API",
        Version = "v1",
        Description = "API for managing catalog items, brands, and types.",
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

builder.Services.AddPersistent(builder.Configuration);
builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection("CatalogSettings"));
builder.Services.AddConsulClient(builder.Configuration);

var app = builder.Build();

var registeredUrl = urls.Split(';').First();
// Register the service with Consul, providing a default URL in case none is configured
app.UseConsulRegister(
    builder.Configuration,
    app.Lifetime,
    defaultUrl:registeredUrl); // Default URL if no address is configured

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1"); });
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

app.MigrateDbContext<CatalogDbContext>((context, services) =>
{
    // Seed data if needed
    CatalogContextSeed
        .SeedAsync(context, services.GetRequiredService<ILogger<CatalogContextSeed>>())
        .Wait();
});

app.Run();