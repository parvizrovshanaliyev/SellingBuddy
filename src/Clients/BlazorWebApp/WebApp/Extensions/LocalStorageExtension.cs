using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace WebApp.Extensions;

public static class LocalStorageExtension
{
    public static string GetUsername(this ISyncLocalStorageService localStorageService)
    {
        return localStorageService.GetItem<string>("username");
    }
    
    public static async Task<string> GetUsername(this ILocalStorageService localStorageService)
    {
        return await localStorageService.GetItemAsync<string>("username");
    }
    
    public static void SetUsername(this ISyncLocalStorageService localStorageService, string username)
    {
        localStorageService.SetItem("username", username);
    }
    
    public static async Task SetUsername(this ILocalStorageService localStorageService, string username)
    {
        await localStorageService.SetItemAsync("username", username);
    }
    
    public static void RemoveUsername(this ISyncLocalStorageService localStorageService)
    {
        localStorageService.RemoveItem("username");
    }
    
    public static async Task RemoveUsername(this ILocalStorageService localStorageService)
    {
        await localStorageService.RemoveItemAsync("username");
    }
    
    public static string GetToken(this ISyncLocalStorageService localStorageService)
    {
        return localStorageService.GetItem<string>("token");
    }
    
    public static async Task<string> GetToken(this ILocalStorageService localStorageService)
    {
        return await localStorageService.GetItemAsync<string>("token");
    }
    
    public static void SetToken(this ISyncLocalStorageService localStorageService, string token)
    {
        localStorageService.SetItem("token", token);
    }
    
    public static async Task SetToken(this ILocalStorageService localStorageService, string token)
    {
        await localStorageService.SetItemAsync("token", token);
    }
}