using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Localization;

namespace WTA.Infrastructure.Localization;

public class JsonStringLocalizer : IStringLocalizer
{
    public Lazy<Dictionary<string, string>> _dictionary;

    public JsonStringLocalizer()
    {
        _dictionary = new Lazy<Dictionary<string, string>>(() =>
        {
            var result = new Dictionary<string, string>();
            WebApp.ModuleAssemblies
               .ForEach(assembly =>
               {
                   var filePath = $"{assembly.GetName().Name}.Resources.{Thread.CurrentThread.CurrentCulture.Name}.json";
                   using var stream = assembly.GetManifestResourceStream(filePath);
                   if (stream is not null)
                   {
                       using var jdoc = JsonDocument.Parse(stream);
                       var keyValues = jdoc.Deserialize<Dictionary<string, string>>();
                       foreach (var item in keyValues!)
                       {
                           result[item.Key] = item.Value;
                       }
                   }
               });
            return result;
        });
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var actualValue = this[name];
            return !actualValue.ResourceNotFound
                ? new LocalizedString(name, string.Format(CultureInfo.InvariantCulture, actualValue.Value, arguments), false)
                : actualValue;
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return _dictionary.Value.Select(o => new LocalizedString(o.Key, o.Value));
    }

    private string GetString(string key)
    {
        if (_dictionary.Value.TryGetValue(key, out var value))
        {
            return value;
        }
        if (key.Contains('.') && _dictionary.Value.TryGetValue(key.Substring(key.IndexOf('.') + 1), out var value2))
        {
            return value2;
        }
        return key;
    }
}
