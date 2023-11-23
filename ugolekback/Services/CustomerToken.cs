﻿using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ugolekback;

namespace Ugolek.Backend.Web.Services
{
    public class TokenServiceOptions
    {
        public required string ISSUER { get; init; }
        public required string AUDIENCE { get; init; }
        public required string KEY { get; init; }
    }

    public interface ICustomerToken
    {
        public string GenerateToken(string userAddress);
    }
    public class CustomerToken: ICustomerToken
    {
        public readonly TokenServiceOptions options;
        public CustomerToken(IOptions<TokenServiceOptions> options)
        {
            this.options = options.Value;
        }

        public SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.KEY));  //возвращает ключ безопасности, который применяется для генерации токена

        public string GenerateToken(string userAddress)
        {
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, userAddress) };
            var jwt = new JwtSecurityToken(
                    issuer: options.ISSUER,
                    audience: options.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(10)), 
                    signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }
    }
}
