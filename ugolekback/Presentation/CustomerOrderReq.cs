using Ugolek.Backend.Web.Application.Features.Orders;

namespace Ugolek.Backend.Web.Presentation;

public class CustomerOrderReq {
    public string Settlement { get; init; }

    public string Street { get; init; }

    public string House { get; init; }

    public List<OrderItemDto> OrderItems { get; init; }
}