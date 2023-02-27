namespace WTA.Application.Abstractions;

public interface IRepository<T> where T : class
{
    ValueTask<T?> FindAsync(Guid id, CancellationToken cancellationToken = default);

    IQueryable<T> AsNoTracking();

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(T[] entities, CancellationToken cancellationToken = default);

    IQueryable<T> Query();

    void Remove(T entity);

    void RemoveAsync(T[] entities);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
