using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WTA.Application.Extensions;

public static class PropertyInfoExtensions
{
    public static string GetDisplayName(this PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<DisplayAttribute>()?.Name ?? propertyInfo.Name;
    }
}
