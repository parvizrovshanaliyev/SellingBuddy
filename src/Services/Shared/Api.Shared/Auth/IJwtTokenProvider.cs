using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Shared.Auth;

public interface IJwtTokenProvider
{
    string GenerateToken(string username, string role);
}

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtTokenProvider(IOptions<JwtConfig> jwtConfig)
    {
        if (jwtConfig == null)
        {
            throw new ArgumentNullException(nameof(jwtConfig));
        }

        _secretKey = jwtConfig.Value.Secret ?? throw new ArgumentNullException(nameof(jwtConfig.Value.Secret));
        _issuer = jwtConfig.Value.Issuer ?? throw new ArgumentNullException(nameof(jwtConfig.Value.Issuer));
        _audience = jwtConfig.Value.Audience ?? throw new ArgumentNullException(nameof(jwtConfig.Value.Audience));
    }

    public string GenerateToken(string username, string role)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(10);

        var token = new JwtSecurityToken(
            _issuer,
            _audience,
            claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}