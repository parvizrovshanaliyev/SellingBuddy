using IdentityService.Api.Application.Models;

namespace IdentityService.Api.Application.Services;

public interface IIdentityService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}