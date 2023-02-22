namespace WTA.Core.Extensions;

public static class EnumerableExtensions
{
    /// <summary>
    /// IEnumerable 的 Foreach 扩展
    /// </summary>
    public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
    {
        values.ToList().ForEach(action);
    }

    /// <summary>
    /// 根据条件查询
    /// </summary>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Func<T, bool> predicate)
    {
        return condition ? source.Where(predicate) : source;
    }
}
