using Ugolek.Backend.Web.Application.Features.Coals;
using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Application.Features.Orders;

public record OrderItem : IEntity {
    public long Id { get; set; }

    /// <summary>
    /// Итоговая цена за позицию в заказе, в копейках.
    /// </summary>
    public required int Price { get; set; }

    /// <summary>
    /// Масса угля в кг.
    /// </summary>
    public required int Weight { get; set; }

    public required Coal Coal { get; set; }

    public static OrderItem CreateForPosition(Coal coal, int weight) {
        // цена за тонну делить на 1000 кг умножить на кг
        return new OrderItem {
            Price = coal.Price * weight,
            Weight = weight,
            Coal = coal,
            Id = Random.Shared.NextInt64()
        };
    }
}