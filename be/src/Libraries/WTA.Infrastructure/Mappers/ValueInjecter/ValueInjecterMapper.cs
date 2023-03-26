using System.Collections;
using Omu.ValueInjecter;
using WTA.Application.Abstractions;
using WTA.Application.Extensions;

namespace WTA.Infrastructure.Mappers.ValueInjecter;

public class ValueInjecterMapper : IObjectMapper
{
    public void From<T>(T to, object from)
    {
        try
        {
            to.InjectFrom<DeepInjection>(from);
        }
        catch (Exception ex)
        {
            throw new Exception($"{from.GetType().FullName}映射到${typeof(T).FullName}时失败:{ex.Message},{ex}", ex);
        }
    }

    public T To<T>(object from)
    {
        try
        {
            if (typeof(T).IsGenericType && typeof(T).IsAssignableTo(typeof(IList)) && from is IList fromList)
            {
                var toListType = typeof(T);
                var elementType = typeof(T).GetGenericArguments()[0];
                var toList = (IList)Activator.CreateInstance(typeof(T))!;
                if (fromList.Count > 0)
                {
                    foreach (var item in fromList)
                    {
                        var targt = Activator.CreateInstance(elementType);
                        targt.From(item);
                        toList.Add(targt);
                    }
                }
                return (T)toList;
            }
            return (T)Activator.CreateInstance<T>().InjectFrom<DeepInjection>(from);
        }
        catch (Exception ex)
        {
            throw new Exception($"{from.GetType().FullName}映射到${typeof(T).FullName}时失败:{ex.Message},{ex}", ex);
        }
    }
}
