using System;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Application.Services.Identity;
using WebApp.Domain.User;
using WebApp.Extensions;
using WebApp.Infrastructure.Identity;

namespace WebApp.Infrastructure.Indentity;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ISyncLocalStorageService _syncLocalStorageService;
    private readonly AuthenticationStateProvider _authStateProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdentityService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for making requests.</param>
    /// <param name="syncLocalStorageService">The local storage service for storing and retrieving data.</param>
    /// <param name="authStateProvider">The authentication state provider.</param>
    public IdentityService(HttpClient httpClient, ISyncLocalStorageService syncLocalStorageService,
        AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _syncLocalStorageService = syncLocalStorageService;
        _authStateProvider = authStateProvider;
    }

    /// <summary>
    /// Gets a value indicating whether the user is authenticated.
    /// </summary>
    /// <value>
    /// <c>true</c> if the user is authenticated (i.e., a valid token exists); otherwise, <c>false</c>.
    /// </value>
    public bool IsAuthenticated => !string.IsNullOrEmpty(GetUserToken());

    /// <summary>
    /// Retrieves the username of the currently authenticated user.
    /// </summary>
    /// <returns>The username of the current user.</returns>
    public string GetUserName() => _syncLocalStorageService.GetUsername();

    /// <summary>
    /// Retrieves the authentication token of the currently authenticated user.
    /// </summary>
    /// <returns>The authentication token.</returns>
    public string GetUserToken() => _syncLocalStorageService.GetToken();

    /// <summary>
    /// Logs in the user using the specified username and password.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if login was successful; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="HttpRequestException">Thrown if there is an error with the HTTP request.</exception>
    public async Task<bool> Login(string username, string password)
    {
        try
        {
            // Create the login request
            var request = new UserLoginRequest(username, password);

            // Make the HTTP request to authenticate the user
            var response = await _httpClient.PostGetResponseAsync<UserLoginResponse, UserLoginRequest>("auth/login", request);

            // If the token is returned, login was successful
            if (!string.IsNullOrEmpty(response.UserToken))
            {
                // Store the token and username in local storage
                StoreUserCredentials(response.UserToken, username);

                // Notify the authentication state provider about the successful login
                NotifyAuthenticationState(username);

                // Set the authorization header for future requests
                SetAuthorizationHeader(response.UserToken);

                return true;
            }

            return false;
        }
        catch (HttpRequestException httpEx)
        {
            // Log the HTTP-specific error (optional)
            // _logger.LogError($"Login failed due to HTTP request error: {httpEx.Message}");

            throw new HttpRequestException("An error occurred while making the login request.", httpEx);
        }
        catch (Exception ex)
        {
            // Log the unexpected error (optional)
            // _logger.LogError($"An unexpected error occurred during login: {ex.Message}");

            throw new Exception("An unexpected error occurred during login.", ex);
        }
    }

    /// <summary>
    /// Stores the user credentials (token and username) in local storage.
    /// </summary>
    /// <param name="token">The token returned from the server.</param>
    /// <param name="username">The username of the user.</param>
    private void StoreUserCredentials(string token, string username)
    {
        _syncLocalStorageService.SetToken(token);
        _syncLocalStorageService.SetUsername(username);
    }

    /// <summary>
    /// Notifies the authentication state provider about the successful login.
    /// </summary>
    /// <param name="username">The username of the logged-in user.</param>
    private void NotifyAuthenticationState(string username)
    {
        ((CustomAuthenticationStateProvider)_authStateProvider).NotifyUserAuthentication(username);
    }

    /// <summary>
    /// Sets the authorization header for future HTTP requests.
    /// </summary>
    /// <param name="token">The token to be set in the authorization header.</param>
    private void SetAuthorizationHeader(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Logs out the current user by clearing the stored token and username.
    /// </summary>
    public void Logout()
    {
        // Remove the token and username from local storage
        _syncLocalStorageService.RemoveItem("token");
        _syncLocalStorageService.RemoveItem("username");

        // Notify the authentication state provider about the logout
        ((CustomAuthenticationStateProvider)_authStateProvider).NotifyUserLogout();

        // Clear the authorization header for future requests
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
