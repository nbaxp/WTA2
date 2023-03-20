using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using WTA.Application.Application;
using WTA.Application.Domain;

namespace WTA.Application.Abstractions.Controllers;

public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var typeInfos = App.Assemblies!
            .SelectMany(o => o.GetTypes())
            .Where(o => !o.IsAbstract && o.IsAssignableTo(typeof(IResource)))
            .Select(o => o.GetTypeInfo())
            .ToList();
        foreach (var entityTypeInfo in typeInfos)
        {
            var entityType = entityTypeInfo.AsType();
            var prefix = entityType.Assembly.GetName().Name + ".";
            var typeName = entityType.Name + "Controller";
            if (!feature.Controllers.Any(o => o.FullName!.StartsWith(prefix) && o.Name == typeName))
            {
                var modelType = entityType.Assembly.GetTypes()
                    .FirstOrDefault(o => o.GetCustomAttributes().Any(a => a.GetType() == typeof(ModelAttribute<>).MakeGenericType(entityType))) ?? entityType;
                var listModelType = entityType.Assembly.GetTypes()
                    .FirstOrDefault(o => o.GetCustomAttributes().Any(a => a.GetType() == typeof(ListModelAttribute<>).MakeGenericType(entityType))) ?? entityType;
                var searchModelType = entityType.Assembly.GetTypes()
                    .FirstOrDefault(o => o.GetCustomAttributes().Any(a => a.GetType() == typeof(SearchModelAttribute<>).MakeGenericType(entityType))) ?? entityType;
                //var indexModelType = typeof(PaginationModel<,>).MakeGenericType(searchModelType, listModelType);
                var typeInfo = typeof(GenericController<,,,>).MakeGenericType(entityType, modelType, searchModelType, listModelType).GetTypeInfo();
                feature.Controllers.Add(typeInfo);
            }
        }
        stopWatch.Stop();
        Console.WriteLine("init controllers total seconds:" + stopWatch.ElapsedMilliseconds / 1000.0);
    }
}
