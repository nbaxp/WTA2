using Microsoft.Extensions.DependencyInjection;
using WTA.Application;
using WTA.Core.Abstractions;

namespace WTA.Core.Extensions;

public static class ObjectMapperExtensions
{
    public static T To<T>(this object from)
    {
        using var scope = App.Services!.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IObjectMapper>().To<T>(from);
    }

    public static T From<T>(this T target, object from)
    {
        target.From(from);
        return target;
    }
}
