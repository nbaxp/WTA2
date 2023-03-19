namespace WTA.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BaseContextAttribute : Attribute
{
    public Type DbContextType { get; set; } = null!;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DbContextAttribute<T> : BaseContextAttribute
{
    public DbContextAttribute()
    {
        this.DbContextType = typeof(T);
    }
}
