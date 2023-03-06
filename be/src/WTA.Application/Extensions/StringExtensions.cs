using System.Text.RegularExpressions;

namespace WTA.Application.Extensions;

public static class StringExtensions
{
    public static string ToLowerCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input) || !char.IsUpper(input[0]))
        {
            return input;
        }
        var chars = input.ToCharArray();
        FixCasing(chars);
        return new string(chars);
    }

    public static string ToSlugify(this string input)
    {
        return Regex.Replace(input.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }

    public static string ToUnderline(this string input)
    {
        return Regex.Replace(input.ToString()!, "([a-z])([A-Z])", "$1_$2").ToLowerInvariant();
    }

    public static string TrimEnd(this string input, string value)
    {
        return input.EndsWith(value) ? input.Substring(0, input.Length - value.Length) : input;
    }

    public static string TrimEndOptions(this string input)
    {
        var value = "Options";
        return input.EndsWith(value) ? input.Substring(0, input.Length - value.Length) : input;
    }

    private static void FixCasing(Span<char> chars)
    {
        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            var hasNext = i + 1 < chars.Length;

            // Stop when next char is already lowercase.
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                // If the next char is a space, lowercase current char before exiting.
                if (chars[i + 1] == ' ')
                {
                    chars[i] = char.ToLowerInvariant(chars[i]);
                }

                break;
            }

            chars[i] = char.ToLowerInvariant(chars[i]);
        }
    }
}
