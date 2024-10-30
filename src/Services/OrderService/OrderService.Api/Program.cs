using System.Reflection;
using Api.Shared.Extensions;
using Microsoft.AspNetCore.Builder;

namespace OrderService.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.AddSharedAppSettingsAndEnvironmentVariables(args);
        
        builder.Services.AddServices(builder.Configuration, Assembly.GetExecutingAssembly().GetName().Name);

        var app = builder.Build();

        app.UseServices(builder.Configuration, app.Lifetime, app.Environment);

        app.Run();
    }
}