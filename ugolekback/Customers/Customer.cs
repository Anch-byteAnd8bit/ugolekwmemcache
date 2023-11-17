using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using ugolekback.Core;
using ugolekback.EmailF;
using ugolekback.Services;

namespace ugolekback.CustomerF;

public class Customer : IEntity {
    public required long Id { get; set; }
 
    public required string Email { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }

    public string? House { get; set; }
}

public class CustomerVerificationInfo
{
    public string VerificationCode { get; set; }
    public long CustomerId { get; set; }
}

public static class CustomerRepositoryExtensions {
    public static Customer? GetCustomerByEmail(this IRepository<Customer> repo, string emailAddress) {
        return repo.Query()
            .SingleOrDefault(x => x.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));
    }

}


public class CustomerService {
    private readonly IRepository<Customer> customers;

    private readonly IEmailSender emailSender;

    private readonly IVerificationCodeGenerator verificationCodeGenerator;

    private readonly IMemoryCache memoryCache;

    public CustomerService(
        IRepository<Customer> customers,
        IEmailSender emailSender,
        IVerificationCodeGenerator verificationCodeGenerator,
        IMemoryCache memoryCache
    ) {
        this.customers = customers;
        this.emailSender = emailSender;
        this.verificationCodeGenerator = verificationCodeGenerator;
        this.memoryCache = memoryCache;
    }

    public void SaveCodeToCache(string code, Customer customer)
    {
        CustomerVerificationInfo verificationInfo = new() { VerificationCode = code, CustomerId = customer.Id };       
        string key = $"customer-verify-{customer.Id}";
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        memoryCache.Set(key, verificationInfo, cacheEntryOptions);
    }

    public async Task SendToCustomerVerificationCode(string emailAddress, CancellationToken cancellation = default) {
        if (customers.GetCustomerByEmail(emailAddress) is not {  } customer) {
            customer = customers.Insert(new() {Id = Random.Shared.NextInt64(), Email = emailAddress,});
        }

        var verificationCode = verificationCodeGenerator.GetCode();
        SaveCodeToCache(verificationCode, customer);
        await emailSender.SendEmailAsync(customer.Email, $"Код подтверждения: {verificationCode}", cancellation);
    }

    public bool CheckCode(CustomerVerifyRequest req)
    {
        if (customers.GetCustomerByEmail(req.Address) is { } customer)
        {
            var verificationInfo = memoryCache.Get<CustomerVerificationInfo>($"customer-verify-{customer.Id}");
            return req.Code == verificationInfo?.VerificationCode;
        }
        return false;
        
    }
}