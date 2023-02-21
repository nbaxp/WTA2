using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using WTA.Application.Domain;

namespace WTA.Application.Abstractions.Controllers;

public class GenericControllerRouteConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.IsGenericType && controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericController<,,,>))
        {
            var genericType = controller.ControllerType.GenericTypeArguments[0];
            var attribute = genericType.GetCustomAttributes(true).FirstOrDefault(o => o.GetType().IsAssignableTo(typeof(GroupAttribute))) as GroupAttribute;
            var routeTemplate = "[controller]/[action]";
            if (attribute != null && !string.IsNullOrEmpty(attribute.Group))
            {
                routeTemplate = $"{attribute.Group}/{routeTemplate}";
            }
            controller.Selectors.Add(new SelectorModel
            {
                AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(routeTemplate)),
            });
        }
    }
}
