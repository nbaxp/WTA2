using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Application;

public enum UIType
{
    Select,
    MultiSelect,
}

[AttributeUsage(AttributeTargets.Property)]
public class UIAttribute : UIHintAttribute
{
    public UIAttribute(UIType type) : base(type.ToString())
    {
    }
}
