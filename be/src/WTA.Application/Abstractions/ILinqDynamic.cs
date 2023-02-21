namespace WTA.Core.Abstractions;

public interface ILinqDynamic
{
    IQueryable<T> Where<T>(IQueryable<T> source, string queryString, object[] args);

    IQueryable<TEntity> Where<TEntity, TModel>(IQueryable<TEntity> source, TModel model);

    IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args);
}
