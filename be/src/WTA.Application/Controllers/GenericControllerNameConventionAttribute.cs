using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WTA.Core.Domain;

namespace WTA.Core.Application.Controllers;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class GenericControllerNameConventionAttribute : Attribute, IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType || controller.ControllerType.BaseType!.IsGenericType)
        {
            var controllerType = controller.ControllerType.IsGenericType ? controller.ControllerType : controller.ControllerType.BaseType!;
            if (controllerType.GetGenericTypeDefinition() == typeof(GenericController<,,,>))
            {
                var entityType = controllerType.GenericTypeArguments[0];
                if (controller.ControllerName != entityType.Name)
                {
                    controller.ControllerName = entityType.Name;
                }
                var groupName = entityType.GetCustomAttribute<GroupAttribute>()?.Area;
                if (!string.IsNullOrEmpty(groupName))
                {
                    controller.ApiExplorer.GroupName = groupName;
                }
            }
        }
    }
}
