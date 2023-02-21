using System.Text.Json;
using System.Text.RegularExpressions;

namespace WTA.Application.Abstractions.Json;

public class UnderlineJsonNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return ToUnderline(name);
    }

    public static string ToUnderline(string name)
    {
        return Regex.Replace(name.ToString()!, "([a-z])([A-Z])", "$1_$2").ToLowerInvariant();
    }
}
