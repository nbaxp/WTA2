using Microsoft.Extensions.DependencyInjection;
using WTA.Core.Abstractions;

namespace WTA.Core.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> Where<T>(this IQueryable<T> source, string queryString, params object[] args)
    {
        using var scope = App.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.Where(source, queryString, args);
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] args)
    {
        using var scope = App.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.OrderBy(source, ordering, args);
    }

    public static IQueryable<TEntity> Where<TEntity, TModel>(this IQueryable<TEntity> source, TModel model)
    {
        using var scope = App.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.Where(source, model);
    }
}
