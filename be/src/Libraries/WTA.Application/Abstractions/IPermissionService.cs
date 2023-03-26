namespace WTA.Application.Abstractions;

public interface IPermissionService
{
    bool HasPermission(string userName, string permission);
}
