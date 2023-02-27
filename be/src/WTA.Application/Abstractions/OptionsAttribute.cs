namespace WTA.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public class OptionsAttribute : Attribute
{
    public OptionsAttribute(string? section = null)
    {
        Section = section;
    }

    public string? Section { get; }
}
