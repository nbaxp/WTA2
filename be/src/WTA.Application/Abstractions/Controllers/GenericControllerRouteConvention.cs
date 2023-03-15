using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WTA.Application.Domain;
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
            var moduleGroup = moduleName?.Substring(moduleName.LastIndexOf('.') + 1);
            var groupAttribute = genericType.GetCustomAttributes().FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(IGroupAttribute))) as IGroupAttribute;
            var routeTemplate = $"{moduleGroup?.ToSlugify()}/";
            if (groupAttribute != null)
            {
                routeTemplate += $"{groupAttribute?.Name.ToSlugify()}/";
            }
            routeTemplate += "[controller]/[action]";
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routeTemplate)),
            });
        }
    }
}
