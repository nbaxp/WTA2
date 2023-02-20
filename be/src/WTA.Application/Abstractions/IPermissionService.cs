namespace WTA.Core.Abstractions;

public interface IPermissionService
{
    bool HasPermission(string userName, string permission);
}