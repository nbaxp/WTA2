namespace WTA.Core.Application;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public class ModuleAttribute : Attribute
{
    public ModuleAttribute(string name, int order = 0)
    {
        this.Name = name;
        this.Order = order;
    }

    public string Name { get; }
    public int Order { get; }
}
