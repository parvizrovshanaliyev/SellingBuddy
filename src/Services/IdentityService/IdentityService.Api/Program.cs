using System.Reflection;
using Api.Shared.Auth;
using Api.Shared.Extensions;
using IdentityService.Api.Application.Services;
using Microsoft.OpenApi.Models;

namespace IdentityService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSharedAppSettingsAndEnvironmentVariables(args);
        
        builder.Services.AddServices(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);
        
        builder.Services.AddScoped<IIdentityService, IdentityService.Api.Application.Services.IdentityService>();
        
        var app = builder.Build();
        
        app.UseServices(builder.Configuration, app.Lifetime, app.Environment);

        app.Run();
    }
}