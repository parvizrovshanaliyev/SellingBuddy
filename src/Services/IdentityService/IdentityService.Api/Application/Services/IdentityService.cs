using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Api.Application.Services;

public class IdentityService : IIdentityService
{
    public Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        // db process will be here
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Username),
            new Claim(ClaimTypes.Role, "User")
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(10);
        
        var token = new JwtSecurityToken(
            "issuer",
            "audience",
            claims,
            expires: expires,
            signingCredentials: credentials
        );
        
        var tokenHandler = new JwtSecurityTokenHandler();
        
        return Task.FromResult(new LoginResponse
        {
            Username = request.Username,
            UserToken = tokenHandler.WriteToken(token)
        });
    }
}