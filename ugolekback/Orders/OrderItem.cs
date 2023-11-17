using ugolekback.Coals.Model;

namespace ugolekback.OrderF;

public record OrderItem
{
    public int Id { get; set; }
    public decimal Price { get; set; }
    public int Weight { get; set; }
    public required Coal Coal { get; set; }
        
}
public class ItemTemp
{
    public int Id { get; set; }
    public int Weight { get; set; }
}