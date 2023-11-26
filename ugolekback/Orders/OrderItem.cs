using Ugolek.Backend.Web.Coals.Model;
using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Orders;

public record OrderItem: IEntity
{
    public long Id { get; set; }
    public decimal Price { get; set; }
    public int Weight { get; set; }
    public required Coal Coal { get; set; }
        
}
public class ItemTemp: IEntity
{
    public long Id { get; set; }
    public int Weight { get; set; }
}