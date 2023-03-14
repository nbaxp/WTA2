using System.Diagnostics;
using System.Globalization;
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

    public static string[] ToValues(this string input)
    {
        return input?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    }

    public static int ToInt(this string value)
    {
        return int.Parse(value, CultureInfo.InvariantCulture);
    }

    public static long ToLong(this string value)
    {
        return long.Parse(value, CultureInfo.InvariantCulture);
    }

    public static string RunCommand(this string command)
    {
        try
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = OperatingSystem.IsWindows() ? "cmd" : "sh",
                    Arguments = $@"{(OperatingSystem.IsWindows() ? "/C" : "-c")} {command}",
                },
            };
            process.Start();
            var result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception($"{nameof(RunCommand)}:{command}", ex);
        }
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
