namespace Ugolek.Backend.Web.Application.Services;

public interface ICustomerToken
{
    public string GenerateToken(string userAddress);
}