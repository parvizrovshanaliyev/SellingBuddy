using Microsoft.AspNetCore.Http;

namespace BasketService.Api.Core.Infrastructure.Services;

public class IdentityService : IIdentityService
{
     private readonly IHttpContextAccessor _httpContextAccessor;

     public IdentityService(IHttpContextAccessor httpContextAccessor)
     {
          _httpContextAccessor = httpContextAccessor;
     }

     public string GetUserName()
     {
          return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
     }
}