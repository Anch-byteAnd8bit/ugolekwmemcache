using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Ugolek.Backend.Web.Application.Services.CustomerTokens;

public class TokenServiceOptions {
    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required string Key { get; init; }

    /// <summary>
    /// Ключ безопасности, который применяется для генерации токена.
    /// </summary>
    public SymmetricSecurityKey SecurityKey => new(Encoding.UTF8.GetBytes(Key));
}