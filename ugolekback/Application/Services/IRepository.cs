using Ugolek.Backend.Web.Core;

namespace Ugolek.Backend.Web.Application.Services;

public interface IRepository<T> where T : class, IEntity {
    T? GetById(long id);

    ICollection<T> GetMany();

    IQueryable<T> Query();

    T Insert(T entity);
}