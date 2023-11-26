using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Application.Features.Orders;

public class OrderItemDto: IEntity
{
    public long Id { get; set; }
    public int Weight { get; set; }
}