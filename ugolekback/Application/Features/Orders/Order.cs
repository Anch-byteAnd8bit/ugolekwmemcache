using Ugolek.Backend.Web.Application.Features.Customers;
using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Application.Features.Orders;

public class Order : IEntity
{
    public long Id { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal OrderPrice { get; set; }
    public required Customer Customer { get; set; }

    public required List<OrderItem> OrderItems { get; set; }
}