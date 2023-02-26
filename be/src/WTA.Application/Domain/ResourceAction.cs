namespace WTA.Application.Domain;

public class ResourceAction
{
    private ResourceAction(string name)
    {
        this.Name = name;
    }

    public string Name { get; }

    public static ResourceAction List()
    {
        return new ResourceAction(nameof(List));
    }

    public static ResourceAction Create()
    {
        return new ResourceAction(nameof(Create));
    }

    public static ResourceAction Update()
    {
        return new ResourceAction(nameof(Update));
    }

    public static ResourceAction Delete()
    {
        return new ResourceAction(nameof(Delete));
    }

    public static ResourceAction Custom(string name)
    {
        return new ResourceAction(name);
    }
}
