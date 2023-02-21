using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using WTA.Application.Domain;

namespace WTA.Application.Abstractions.Controllers;

public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var typeInfos = Assembly
          .GetAssembly(typeof(BaseEntity))!.GetTypes()
          //.Where(o => !o.IsAbstract && o.IsAssignableTo(typeof(BaseEntity)))//根据实体类型
          .Where(o => o.CustomAttributes.Any(o => o.AttributeType.IsAssignableTo(typeof(GroupAttribute))))//根据注解获取
          .Select(o => o.GetTypeInfo())
          .ToList();
        foreach (var entityTypeInfo in typeInfos)
        {
            var entityType = entityTypeInfo.AsType();
            var typeName = entityType.Name + "Controller";
            if (!feature.Controllers.Any(o => o.Name == typeName &&
                o.BaseType != null &&
                o.BaseType.IsGenericType &&
                o.BaseType.GetGenericTypeDefinition() == typeof(GenericController<,,,>)))
            {
                var searchModelType = typeof(PaginationViewModel<>).MakeGenericType(entityType);
                feature.Controllers.Add(typeof(GenericController<,,,>).MakeGenericType(entityType, entityType, entityType, searchModelType).GetTypeInfo());
            }
        }
    }
}
