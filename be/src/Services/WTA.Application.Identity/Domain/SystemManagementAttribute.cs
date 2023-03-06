using WTA.Application.Domain;

namespace WTA.Application.Identity.Domain;

[AttributeUsage(AttributeTargets.Class)]
public class SystemManagementAttribute : Attribute, IGroup
{
    public string Name => nameof(SystemManagement);

    public int DisplayOrder => -1;

    public string Icon => "setting";
}
