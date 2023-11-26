using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ugolek.Backend.Web.Application.Services.CustomerTokens;

public class CustomerToken : ICustomerToken
{
    public static TokenServiceOptions options;

    public CustomerToken(IOptions<TokenServiceOptions> options) {
        CustomerToken.options = options.Value;
    }

    public string GenerateToken(string userAddress) {
        var claims = new List<Claim> { new(ClaimTypes.Name, userAddress) };

        var jwt = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)), 
            signingCredentials: new SigningCredentials(options.SecurityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);

    }

    public static string? GetCurrentEmail(System.Security.Principal.IIdentity? _identity)
    {
        // Убедимся, что это ClaimsIdentity, а не ClaimsPrincipal.
        if (_identity is ClaimsIdentity identity)
        {
            string? email = identity.FindFirst(ClaimTypes.Name)?.Value;
            return email;
        }
        return null;
    }
}