using Microsoft.AspNetCore.Http;

namespace BasketService.Api.Core.Application.Services;

public interface IIdentityService
{
     string GetUserName();
}
