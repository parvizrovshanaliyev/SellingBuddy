using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Extensions;

namespace WebApp.Utils;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationState _anonymous;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomAuthenticationStateProvider"/> class.
    /// </summary>
    /// <param name="localStorageService">The local storage service for retrieving stored tokens and usernames.</param>
    /// <param name="httpClient">The HTTP client for setting the authorization header.</param>
    public CustomAuthenticationStateProvider(ILocalStorageService localStorageService, HttpClient httpClient)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClient;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    /// <summary>
    /// Gets the current authentication state of the user.
    /// </summary>
    /// <returns>The authentication state of the user, either authenticated or anonymous.</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Try to retrieve the token from local storage
        string token = await _localStorageService.GetTokenAsync();
        
        // If no token is found, return anonymous state
        if (string.IsNullOrWhiteSpace(token))
        {
            return _anonymous;
        }

        // Try to retrieve the username from local storage
        string username = await _localStorageService.GetUsernameAsync();

        // If the username is not found, fallback to an empty string
        if (string.IsNullOrWhiteSpace(username))
        {
            username = "Unknown";
        }

        // Create the ClaimsPrincipal with the username
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }, "jwtAuthType"));

        // Set the authorization header for subsequent HTTP requests
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return new AuthenticationState(claimsPrincipal);
    }

    /// <summary>
    /// Notifies the authentication state provider that a user has been authenticated.
    /// </summary>
    /// <param name="username">The username of the authenticated user.</param>
    public void NotifyUserAuthentication(string username)
    {
        // Create the ClaimsPrincipal for the authenticated user
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }, "jwtAuthType"));

        // Notify the authentication state has changed to authenticated
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(authenticatedUser)));
    }

    /// <summary>
    /// Notifies the authentication state provider that the user has logged out.
    /// </summary>
    public void NotifyUserLogout()
    {
        // Notify the authentication state has changed to anonymous
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }
}
