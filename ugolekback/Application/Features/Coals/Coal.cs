using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Application.Features.Coals;

public class Coal : IEntity {
    public long Id { get; set; } 

    public string Name { get; set; }

    public int Price { get; set; }
}