using ugolekback.Coals.Model;
using ugolekback.Core;

namespace Ugolek.Backend.Web.Services
{
    public static class EndpointConfiguration
    {
        public static void MapCoalEndpoints(this WebApplication app)
        {
            app.MapGet("/coals", (IRepository<Coal> repo) =>
            {
                return repo.GetMany();
            });
        }
    }
}
