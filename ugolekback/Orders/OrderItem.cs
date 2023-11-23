using ugolekback.Coals.Model;
using ugolekback.Core;

namespace ugolekback.OrderF;

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