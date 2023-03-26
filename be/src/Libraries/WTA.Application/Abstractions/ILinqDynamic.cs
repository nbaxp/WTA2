namespace WTA.Application.Abstractions;

public interface ILinqDynamic
{
    IQueryable<T> Where<T>(IQueryable<T> source, string queryString, object[] args);

    IQueryable<TEntity> Where<TEntity, TModel>(IQueryable<TEntity> source, TModel model) where TModel : class;

    IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] args);

    List<TModel> ToList<TEntity, TModel>(IQueryable<TEntity> source) where TModel : class;
}
