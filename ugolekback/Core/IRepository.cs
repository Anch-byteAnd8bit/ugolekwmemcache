namespace ugolekback.Core;

public interface IRepository<T> where T : class, IEntity {
    T? GetById(long id);
    long GetLastId();

    ICollection<T> GetMany();

    IQueryable<T> Query();

    T Insert(T entity);
}