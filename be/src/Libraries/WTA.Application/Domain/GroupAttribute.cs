namespace WTA.Application.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class GroupAttribute : Attribute
{
    public GroupAttribute(string name, string icon)
    {
        this.Name = name;
        this.Icon = icon;
    }

    public string Name { get; }
    public string Icon { get; }
}
