namespace WTA.Application.Application;

public interface IModelAttribute
{
    Type EntityType { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ModelAttribute<T> : Attribute, IModelAttribute
{
    public Type EntityType => typeof(T);
}

[AttributeUsage(AttributeTargets.Class)]
public class ListModelAttribute<T> : Attribute, IModelAttribute
{
    public Type EntityType => typeof(T);
}

[AttributeUsage(AttributeTargets.Class)]
public class SearchModelAttribute<T> : Attribute, IModelAttribute
{
    public Type EntityType => typeof(T);
}
