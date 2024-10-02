using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IdentityService.Api.Application.Models;
using IdentityService.Api.Application.Services;

namespace IdentityService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _identityService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }
}