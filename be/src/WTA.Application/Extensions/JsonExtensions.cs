using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Microsoft.Extensions.Logging;

namespace WTA.Application.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string? ToJson(this object input)
    {
        try
        {
            return JsonSerializer.Serialize(input, _jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            App.Logger?.LogError(ex, ex.Message);
        }
        return null;
    }

    public static T? FromJson<T>(this string value)
    {
        return JsonSerializer.Deserialize<T>(value);
    }
}
