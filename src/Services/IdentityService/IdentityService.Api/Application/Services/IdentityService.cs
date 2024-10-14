using System.Security.Claims;
using Api.Shared.Auth;
using IdentityService.Api.Application.Models;

namespace IdentityService.Api.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public IdentityService(IJwtTokenProvider jwtTokenProvider)
    {
        _jwtTokenProvider = jwtTokenProvider;
    }

    public Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "User")
        };

        var token = _jwtTokenProvider.GenerateToken(request.Username, "User");

        return Task.FromResult(new LoginResponse
        {
            Username = request.Username,
            UserToken = token
        });
    }
}