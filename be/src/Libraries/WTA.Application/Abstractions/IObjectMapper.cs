namespace WTA.Application.Abstractions;

public interface IObjectMapper
{
    void From<T>(T to, object from);

    T To<T>(object from);
}
