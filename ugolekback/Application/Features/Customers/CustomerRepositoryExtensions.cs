using Ugolek.Backend.Web.Application.Services;

namespace Ugolek.Backend.Web.Application.Features.Customers;

public static class CustomerRepositoryExtensions {
    public static Customer? GetCustomerByEmail(this IRepository<Customer> repo, string? emailAddress) {
        return repo.Query()
            .SingleOrDefault(x => x.Email.Equals(emailAddress, StringComparison.InvariantCultureIgnoreCase));
    }
}