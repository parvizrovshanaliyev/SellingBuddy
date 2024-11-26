using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace WebApp.Extensions;

/// <summary>
/// Provides extension methods for handling local storage operations for username and token.
/// </summary>
public static class LocalStorageExtension
{
    #region Username Methods

    /// <summary>
    /// Gets the username from the synchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <returns>The username stored in local storage.</returns>
    public static string GetUsername(this ISyncLocalStorageService localStorageService)
    {
        return localStorageService.GetItem<string>("username");
    }

    /// <summary>
    /// Gets the username from the asynchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <returns>The username stored in local storage.</returns>
    public static async Task<string> GetUsernameAsync(this ILocalStorageService localStorageService)
    {
        return await localStorageService.GetItemAsync<string>("username");
    }

    /// <summary>
    /// Sets the username in the synchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <param name="username">The username to store.</param>
    public static void SetUsername(this ISyncLocalStorageService localStorageService, string username)
    {
        localStorageService.SetItem("username", username);
    }

    /// <summary>
    /// Sets the username in the asynchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <param name="username">The username to store.</param>
    public static async Task SetUsernameAsync(this ILocalStorageService localStorageService, string username)
    {
        await localStorageService.SetItemAsync("username", username);
    }

    /// <summary>
    /// Removes the username from the synchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    public static void RemoveUsername(this ISyncLocalStorageService localStorageService)
    {
        localStorageService.RemoveItem("username");
    }

    /// <summary>
    /// Removes the username from the asynchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    public static async Task RemoveUsernameAsync(this ILocalStorageService localStorageService)
    {
        await localStorageService.RemoveItemAsync("username");
    }

    #endregion

    #region Token Methods

    /// <summary>
    /// Gets the token from the synchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <returns>The token stored in local storage.</returns>
    public static string GetToken(this ISyncLocalStorageService localStorageService)
    {
        return localStorageService.GetItem<string>("token");
    }

    /// <summary>
    /// Gets the token from the asynchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <returns>The token stored in local storage.</returns>
    public static async Task<string> GetTokenAsync(this ILocalStorageService localStorageService)
    {
        return await localStorageService.GetItemAsync<string>("token");
    }

    /// <summary>
    /// Sets the token in the synchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <param name="token">The token to store.</param>
    public static void SetToken(this ISyncLocalStorageService localStorageService, string token)
    {
        localStorageService.SetItem("token", token);
    }

    /// <summary>
    /// Sets the token in the asynchronous local storage.
    /// </summary>
    /// <param name="localStorageService">The local storage service.</param>
    /// <param name="token">The token to store.</param>
    public static async Task SetTokenAsync(this ILocalStorageService localStorageService, string token)
    {
        await localStorageService.SetItemAsync("token", token);
    }

    #endregion
}
