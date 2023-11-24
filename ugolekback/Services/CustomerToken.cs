using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ugolekback;
using ugolekback.CustomerF;

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
        public static TokenServiceOptions options;
        public CustomerToken(IOptions<TokenServiceOptions> options)
        {
            CustomerToken.options = options.Value;
        }

        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
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

    //public class JWTMiddleware
    //{
    //    private readonly RequestDelegate _next;
    //    private readonly IConfiguration _configuration;
    //    private readonly Customer _user;

    //    public JWTMiddleware(RequestDelegate next, IConfiguration configuration, Customer user)
    //    {
    //        _next = next;
    //        _configuration = configuration;
    //        _user = user;
    //    }

    //    public async Task Invoke(HttpContext context)
    //    {
    //        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

    //        if (token != null)
    //            attachAccountToContext(context, token);

    //        await _next(context);
    //    }

    //    private void attachAccountToContext(HttpContext context, string token)
    //    {
    //        try
    //        {
    //            var tokenHandler = new JwtSecurityTokenHandler();
    //            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
    //            tokenHandler.ValidateToken(token, new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                IssuerSigningKey = new SymmetricSecurityKey(key),
    //                ValidateIssuer = true,
    //                ValidateAudience = true,
    //                ClockSkew = TimeSpan.Zero
    //            }, out SecurityToken validatedToken);

    //            var jwtToken = (JwtSecurityToken)validatedToken;
    //            var accountId = jwtToken.Claims.First(x => x.Type == "id").Value;

    //            // attach account to context on successful jwt validation
    //            context.Items["User"] = _user.Email;
    //        }
    //        catch
    //        {
                
    //        }
    //    }
    //}
}
