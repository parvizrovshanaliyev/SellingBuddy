using System.Net.Http;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Domain.User;
using WebApp.Extensions;
using WebApp.Utils;

namespace WebApp.Application.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly ISyncLocalStorageService _syncLocalStorageService;
    private readonly AuthenticationStateProvider _authStateProvider;
    
    public IdentityService(HttpClient httpClient, ISyncLocalStorageService syncLocalStorageService, AuthenticationStateProvider authStateProvider)
    {
        _httpClient = httpClient;
        _syncLocalStorageService = syncLocalStorageService;
        _authStateProvider = authStateProvider;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(GetUserToken());

    public string GetUserName()
    {
        return _syncLocalStorageService.GetUsername();
    }

    public string GetUserToken()
    {
        return _syncLocalStorageService.GetToken();
    }

    public async Task<bool> Login(string username, string password)
    {
        var request = new UserLoginRequest(username, password);
        
        var response= await _httpClient.PostGetResponseAsync<UserLoginResponse,UserLoginRequest>("auth/login", request);
        
        if(!string.IsNullOrEmpty(response.Token))
        {
            _syncLocalStorageService.SetToken(response.Token);
            _syncLocalStorageService.SetUsername(username);
            ((CustomAuthenticationStateProvider)_authStateProvider).NotifyUserAuthentication(username);
            
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.Token);
             
            return true;
        }
        
        return false;
    }

    public void Logout()
    {
        _syncLocalStorageService.RemoveItem("token");
        _syncLocalStorageService.RemoveItem("username");
        
        ((CustomAuthenticationStateProvider)_authStateProvider).NotifyUserLogout();
        
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}