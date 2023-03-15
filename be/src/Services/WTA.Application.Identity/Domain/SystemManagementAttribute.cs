using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class SystemManagementAttribute : Attribute, IGroupAttribute
{
    public int DisplayOrder => -1;
    public string Icon => "setting";
    public string Name => nameof(SystemManagement);
}
