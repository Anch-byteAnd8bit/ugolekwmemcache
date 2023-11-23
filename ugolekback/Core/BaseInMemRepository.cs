﻿namespace ugolekback.Core;

public class BaseInMemRepository<T> : IRepository<T> where T : class, IEntity {
    private readonly ICollection<T> entites;

    public BaseInMemRepository() {
        entites = new List<T>();
    }

    public BaseInMemRepository(IEnumerable<T> entites) {
        this.entites = entites.ToList();
    }

    public T? GetById(long id) {
        return entites.SingleOrDefault(x => x.Id == id);
    }
    public long GetLastId()
    {
        if (entites.Count == 0) { return 0; }
        return entites.Last<T>().Id;
    }

    public ICollection<T> GetMany() {
        return entites;
    }

    public IQueryable<T> Query() {
        return entites.AsQueryable();
    }

    public T Insert(T entity) {
        entity.Id = Random.Shared.NextInt64(100_000, 1_000_000);

        entites.Add(entity);

        return entity;
    }
}