using Mapster;
using WTA.Application.Abstractions;

namespace WTA.Infrastructure.Mappers.Mapster;

[Service<IObjectMapper>]
public class MapsterMapper : IObjectMapper

{
    public void From<T>(T to, object from)
    {
        from.Adapt(to);
    }

    public T To<T>(object from)
    {
        return from.Adapt<T>();
    }
}
