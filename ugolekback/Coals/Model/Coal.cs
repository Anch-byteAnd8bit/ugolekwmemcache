using ugolekback.Core;

namespace ugolekback.Coals.Model;

public class Coal : IEntity {
    public long Id { get; set; } 

    public string Name { get; set; }

    public decimal Price { get; set; }
}