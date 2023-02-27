using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WTA.Application.Abstractions.Controllers;

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
                var moduleName = entityType.Assembly.GetName().Name;
                var groupName = moduleName?.Substring(moduleName.LastIndexOf('.') + 1);
                if (!string.IsNullOrEmpty(groupName))
                {
                    controller.ApiExplorer.GroupName = groupName;
                }
            }
        }
    }
}
