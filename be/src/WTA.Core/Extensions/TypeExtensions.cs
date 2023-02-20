using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WTA.Core.Extensions;

public static class TypeExtensions
{
    public static string GetDisplayName(this Type type)
    {
        return type.GetCustomAttribute<DisplayAttribute>()?.Name ?? type.Name;
    }
}
