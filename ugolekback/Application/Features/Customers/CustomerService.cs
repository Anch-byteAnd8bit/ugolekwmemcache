using Microsoft.Extensions.Caching.Memory;
using Ugolek.Backend.Web.Application.Services;

namespace Ugolek.Backend.Web.Application.Features.Customers;

public class CustomerService {
    private readonly IRepository<Customer> customers;

    private readonly IEmailSender emailSender;

    private readonly ICustomerVerificationCodePersister customerVerificatioinCodePersister;

    public CustomerService(
        IRepository<Customer> customers,
        IEmailSender emailSender,
        ICustomerVerificationCodePersister customerVerificatioinCodePersister,
        IMemoryCache memoryCache
    ) {
        this.customers = customers;
        this.emailSender = emailSender;
        this.customerVerificatioinCodePersister = customerVerificatioinCodePersister;
    }

    public async Task SendToCustomerVerificationCode(string emailAddress, CancellationToken cancellation = default) {
        if (customers.GetCustomerByEmail(emailAddress) is not {  } customer) {
            customer = customers.Insert(new() {Id = Random.Shared.NextInt64(), Email = emailAddress,});
        }
        var verificationCode = customerVerificatioinCodePersister.GenerateCodeForCustomer(customer.Id);
        await emailSender.SendEmailAsync(customer.Email, $"Код подтверждения: {verificationCode}", cancellation);
    }

}