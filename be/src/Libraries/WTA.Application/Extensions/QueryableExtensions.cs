using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;

namespace WTA.Application.Extensions;

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

    public static IQueryable<TEntity> WhereByModel<TEntity, TModel>(this IQueryable<TEntity> source, TModel model) where TModel : class
    {
        using var scope = App.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.Where(source, model);
    }

    public static List<TModel> ToList<TEntity, TModel>(this IQueryable<TEntity> source) where TModel : class
    {
        using var scope = App.Services!.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<ILinqDynamic>();
        return service.ToList<TEntity, TModel>(source);
    }
}
