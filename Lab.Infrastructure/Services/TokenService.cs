using Lab.Application.Common.Interfaces;
using Lab.Domain.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lab.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _key;
    private readonly DateTime _expires = DateTime.UtcNow.AddDays(7);

    public TokenService(IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key configuration not found");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    public string GenerateAccessToken(IUser user)
    {
        return GenerateToken(user);
    }

    private string GenerateToken(IUser user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? string.Empty),
            new Claim(AppClaimTypes.TenantId, user.TenantId.ToString() ?? string.Empty)
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _expires,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var jwtToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

        return tokenHandler.WriteToken(jwtToken);
    }

}
