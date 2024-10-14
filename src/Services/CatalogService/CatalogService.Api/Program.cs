using System.Net;
using System.Reflection;
using Api.Shared.Consul;
using Api.Shared.Host;
using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfiguredKestrel(builder.Configuration, out var urls);

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