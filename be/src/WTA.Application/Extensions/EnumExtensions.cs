using System.Reflection;

namespace WTA.Core.Extensions;

public static class EnumExtensions
{
    public static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
    {
        return enumValue.GetType().GetMember(enumValue.ToString()).First()
            .GetCustomAttributes<T>(inherit: false)
            .FirstOrDefault();
    }
}