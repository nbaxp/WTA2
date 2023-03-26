using Microsoft.AspNetCore.Routing;
using WTA.Application.Extensions;

namespace WTA.Infrastructure.Routing;

public class SlugifyParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value?.ToString()?.ToSlugify();
    }
}
