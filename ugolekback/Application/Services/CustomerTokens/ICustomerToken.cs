namespace Ugolek.Backend.Web.Application.Services.CustomerTokens;

public interface ICustomerToken
{
    public string GenerateToken(string userAddress);
}