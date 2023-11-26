using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Application.Features.Customers;

public class Customer : IEntity 
{
    public required long Id { get; set; }
 
    public required string Email { get; set; }

    public string? City { get; set; }

    public string? Street { get; set; }

    public string? House { get; set; }

    public void ProvideAddress(string city, string street, string house) {
        ArgumentException.ThrowIfNullOrEmpty(city);
        ArgumentException.ThrowIfNullOrEmpty(street);
        ArgumentException.ThrowIfNullOrEmpty(house);

        City = city;
        Street = street;
        House = house;
    }
}