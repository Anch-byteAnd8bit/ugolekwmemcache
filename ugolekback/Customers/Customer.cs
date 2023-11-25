using Microsoft.Extensions.Caching.Memory;
using Ugolek.Backend.Web.Services;
using ugolekback.Core;
using ugolekback.EmailF;

namespace ugolekback.CustomerF;

public class Customer : IEntity 
{
    public required long Id { get; set; }
 
    public required string Email { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }

    public string? House { get; set; }
}

public static class CustomerRepositoryExtensions {
    public static Customer? GetCustomerByEmail(this IRepository<Customer> repo, string? emailAddress) {
        return repo.Query()
            .SingleOrDefault(x => x.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));
    }
    public static Customer? GetCustomerById(this IRepository<Customer> repo, long customerId)
    {
        return repo.Query()
            .SingleOrDefault(x => x.Id.Equals(customerId));
    }

}

public class CustomerService {
    private readonly IRepository<Customer> customers;

    private readonly IEmailSender emailSender;

    private readonly ICustomerVerificationCodePersister customerVerificatioinCodePersister;

    private readonly IMemoryCache memoryCache;

    public CustomerService(
        IRepository<Customer> customers,
        IEmailSender emailSender,
        ICustomerVerificationCodePersister customerVerificatioinCodePersister,
        IMemoryCache memoryCache
    ) {
        this.customers = customers;
        this.emailSender = emailSender;
        this.customerVerificatioinCodePersister = customerVerificatioinCodePersister;
        this.memoryCache = memoryCache;
    }

    public async Task SendToCustomerVerificationCode(string emailAddress, CancellationToken cancellation = default) {
        if (customers.GetCustomerByEmail(emailAddress) is not {  } customer) {
            customer = customers.Insert(new() {Id = Random.Shared.NextInt64(), Email = emailAddress,});
        }
        var verificationCode = customerVerificatioinCodePersister.GenerateCodeForCustomer(customer.Id);
        await emailSender.SendEmailAsync(customer.Email, $"Код подтверждения: {verificationCode}", cancellation);
    }

}