using System.Collections;
using System.Reflection;
using Omu.ValueInjecter.Injections;
using WTA.Application.Extensions;

namespace WTA.Infrastructure.Mappers.ValueInjecter;

public class DeepInjection : LoopInjection
{
    protected override bool MatchTypes(Type sourceType, Type targetType)
    {
        if (sourceType != typeof(string) &&
            targetType != typeof(string) &&
            sourceType.IsGenericType &&
            targetType.IsGenericType &&
            sourceType.IsAssignableTo(typeof(IEnumerable)) &&
            sourceType.IsAssignableTo(typeof(IEnumerable))
            )
        {
            return true;
        }
        return base.MatchTypes(sourceType, targetType);
    }

    protected override void SetValue(object source, object target, PropertyInfo sp, PropertyInfo tp)
    {
        if (sp.PropertyType != typeof(string) &&
            sp.PropertyType != typeof(string) &&
            sp.PropertyType.IsAssignableTo(typeof(IList)) &&
            tp.PropertyType.IsAssignableTo(typeof(IList)))
        {
            var targetGenericType = tp.PropertyType.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(targetGenericType);
            var addMethod = listType.GetMethod("Add");
            var list = Activator.CreateInstance(listType);
            var sourceList = (IList)sp.GetValue(source)!;
            foreach (var item in sourceList)
            {
                addMethod?.Invoke(list, new[] { Activator.CreateInstance(targetGenericType).From(item) });
            }
            tp.SetValue(target, list);
            return;
        }
        base.SetValue(source, target, sp, tp);
    }
}
