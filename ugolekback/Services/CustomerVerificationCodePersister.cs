using Microsoft.Extensions.Caching.Memory;
using ugolekback.Core;
using ugolekback.CustomerF;


namespace Ugolek.Backend.Web.Services
{
    public class CustomerVerificationInfo
    {
        public string VerificationCode { get; set; }
        public long CustomerId { get; set; }
    }

    public interface ICustomerVerificationCodePersister
    {
        public string GenerateCodeForCustomer(long customerId);
        public bool VerifyCustomerCode(long customerId, string recivedCode);

    }

    public class CustomerVerificationCodePersister : ICustomerVerificationCodePersister
    {
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
            if (customers.GetCustomerById(customerId) is { } customer)
            {
                var verificationInfo = memoryCache.Get<CustomerVerificationInfo>(MakeKey(customerId));
                return recivedCode == verificationInfo?.VerificationCode;
            }
            return false;
        }
  

    }


}
