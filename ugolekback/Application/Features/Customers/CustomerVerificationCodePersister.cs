using Microsoft.Extensions.Caching.Memory;
using Ugolek.Backend.Web.Application.Features.Customers;
using Ugolek.Backend.Web.Application.Services;

namespace Ugolek.Backend.Web.Configuration.Customers;

public class CustomerVerificationCodePersister : ICustomerVerificationCodePersister {
    private readonly IMemoryCache memoryCache;

    private readonly IRepository<Customer> customers;

    public CustomerVerificationCodePersister(IMemoryCache memoryCache, IRepository<Customer> customers)
    {
        this.memoryCache = memoryCache;
        this.customers = customers;
    }

    private string MakeKey(long customerId)
    {
        return $"customer-verify-{customerId}";
    } 

    public string GenerateCodeForCustomer(long customerId)
    {
        var newCode = Random.Shared.Next(1010, 9090).ToString();
        CustomerVerificationInfo verificationInfo = new() { VerificationCode = newCode, CustomerId = customerId };
        string key = MakeKey(customerId);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        memoryCache.Set(key, verificationInfo, cacheEntryOptions);
        return (newCode);
    }

    public bool VerifyCustomerCode(long customerId, string recivedCode)
    {
        if (customers.GetById(customerId) is { } customer)
        {
            var verificationInfo = memoryCache.Get<CustomerVerificationInfo>(MakeKey(customerId));
            return recivedCode == verificationInfo?.VerificationCode;
        }
        return false;
    }
}