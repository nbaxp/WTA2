namespace WTA.Application.Application;

[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
public class ModuleAttribute : Attribute
{
    public ModuleAttribute(string name, int order = 0,string icon="folder")
    {
        this.Name = name;
        this.Order = order;
        this.Icon = icon;
    }

    public string Name { get; }
    public int Order { get; }
    public string Icon { get; }
}
