namespace WTA.Application.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class GroupAttribute : Attribute
{
    public GroupAttribute(string group)
    {
        this.Group = group;
    }

    public string Group { get; }
}
