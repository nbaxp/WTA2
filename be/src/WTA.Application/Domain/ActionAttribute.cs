using WTA.Application.Extensions;

namespace WTA.Application.Domain;

public interface IActionAttribute
{
    string Name { get; }
    bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ActionAttribute : Attribute, IActionAttribute
{
    public ActionAttribute(string name, bool disabled = false)
    {
        this.Name = name;
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class QueryAttribute : Attribute, IActionAttribute
{
    public QueryAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(QueryAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class CreateAttribute : Attribute, IActionAttribute
{
    public CreateAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(CreateAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class UpdateAttribute : Attribute, IActionAttribute
{
    public UpdateAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(UpdateAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class RemoveAttribute : Attribute, IActionAttribute
{
    public RemoveAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(RemoveAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class RestoreAttribute : Attribute, IActionAttribute
{
    public RestoreAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(RestoreAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class DeleteAttribute : Attribute, IActionAttribute
{
    public DeleteAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(DeleteAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ImportAttribute : Attribute, IActionAttribute
{
    public ImportAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(ImportAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

[AttributeUsage(AttributeTargets.Class)]
public class ExportAttribute : Attribute, IActionAttribute
{
    public ExportAttribute(string? name = null, bool disabled = false)
    {
        this.Name = name ?? nameof(ExportAttribute).TrimEndAttribute();
        this.Disabled = disabled;
    }

    public string Name { get; }
    public bool Disabled { get; }
}

