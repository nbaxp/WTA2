using Omu.ValueInjecter;
using WTA.Core.Abstractions;

namespace WTA.Infrastructure.Mappers;

[Service<IObjectMapper>]
public class ValueInjecterMapper : IObjectMapper
{
    public void From<T>(T to, object from)
    {
        from.InjectFrom(from);
    }

    public T To<T>(object from)
    {
        var target = Activator.CreateInstance<T>();
        target.InjectFrom(from);
        return target!;
    }
}
