namespace Ugolek.Backend.Web.Application.Services;

public interface ICustomerVerificationCodePersister
{
    public string GenerateCodeForCustomer(long customerId);
    public bool VerifyCustomerCode(long customerId, string recivedCode);

}