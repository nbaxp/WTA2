using WTA.Application.Extensions;

namespace WTA.Application.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class BaseActionAttribute : Attribute
{
    public BaseActionAttribute()
    {
        this.Name = this.GetType().Name.TrimEndAttribute();
    }

    public string Name { get; set; }
    public bool Disabled { get; set; }
    public string? Icon { get; set; } = "page";
    public int DisplayOrder { get; set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ActionAttribute : BaseActionAttribute
{
    public ActionAttribute(string name, bool disabled = false)
    {
        this.Name = name;
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class QueryAttribute : BaseActionAttribute
{
    public QueryAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(QueryAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CreateAttribute : BaseActionAttribute
{
    public CreateAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(CreateAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class UpdateAttribute : BaseActionAttribute
{
    public UpdateAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(UpdateAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RemoveAttribute : BaseActionAttribute
{
    public RemoveAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(RemoveAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class RestoreAttribute : BaseActionAttribute
{
    public RestoreAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(RestoreAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class DeleteAttribute : BaseActionAttribute
{
    public DeleteAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(DeleteAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ImportAttribute : BaseActionAttribute
{
    public ImportAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(ImportAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class ExportAttribute : BaseActionAttribute
{
    public ExportAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(ExportAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }
}
