using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WTA.Application.Extensions;

namespace WTA.Application.Abstractions.Controllers;

public class GenericControllerRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType && controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericController<,,,>))
        {
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            var moduleName = genericType.Assembly.GetName().Name;
            var groupName = moduleName?.Substring(moduleName.LastIndexOf('.') + 1);
            var routeTemplate = $"{groupName?.ToSlugify()}/[controller]/[action]";
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routeTemplate)),
            });
        }
    }
}
