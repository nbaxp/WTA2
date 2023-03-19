using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Domain;

namespace WTA.Infrastructure.Data;

public class EfRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly DbContext _efDbContext;

    public EfRepository(IServiceProvider serviceProvider)
    {
        var dbContextType = (typeof(T).GetCustomAttribute(typeof(DbContextAttribute<>)) as BaseContextAttribute).DbContextType;
        this._efDbContext = serviceProvider.GetRequiredService(dbContextType) as DbContext;
    }

    public ValueTask<T?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _efDbContext.Set<T>().FindAsync(new object?[] { id, cancellationToken }, cancellationToken: cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _efDbContext.Set<T>().AddAsync(entity, cancellationToken).ConfigureAwait(false);
    }

    public Task AddRangeAsync(T[] entities, CancellationToken cancellationToken = default)
    {
        return _efDbContext.AddRangeAsync(entities, cancellationToken);
    }

    public void Remove(T entity)
    {
        _efDbContext.Remove(entity);
    }

    public void RemoveAsync(T[] entities)
    {
        _efDbContext.RemoveRange(entities);
    }

    public IQueryable<T> Query()
    {
        return _efDbContext.Set<T>();
    }

    public IQueryable<T> AsNoTracking()
    {
        return _efDbContext.Set<T>().AsNoTracking();
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _efDbContext.SaveChangesAsync(cancellationToken);
    }
}
