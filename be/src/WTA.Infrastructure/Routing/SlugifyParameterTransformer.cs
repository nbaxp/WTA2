using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing;

namespace WTA.Infrastructure.Routing;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string TransformOutbound(object? value)
    {
        return Regex.Replace(value?.ToString()!, "([a-z])([A-Z])", "$1-$2").ToLowerInvariant();
    }
}
