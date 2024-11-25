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

    public CustomAuthenticationStateProvider(ILocalStorageService localStorageService, HttpClient httpClient)
    {
        _localStorageService = localStorageService;
        _httpClient = httpClient;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }

    public override async  Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token =await _localStorageService.GetToken();
        
        if (string.IsNullOrWhiteSpace(token))
        {
            return _anonymous;
        }
        
        string username = await _localStorageService.GetUsername();
        
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }, "jwtAuthType"));
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        return new AuthenticationState(claimsPrincipal);
    }
    
    public void NotifyUserAuthentication(string username)
    {
        var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }, "jwtAuthType"));
        
        var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
        
        NotifyAuthenticationStateChanged(authState);
    }
    
    public void NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        
        NotifyAuthenticationStateChanged(authState);
    }
}