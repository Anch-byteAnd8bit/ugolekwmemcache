using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Coals.Model;

public class Coal : IEntity {
    public long Id { get; set; } 

    public string Name { get; set; }

    public decimal Price { get; set; }
}