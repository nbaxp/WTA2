using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WTA.Infrastructure.Swagger;

public class SwaggerFilter : IDocumentFilter, IOperationFilter
{
    private readonly IStringLocalizer _localizer;

    public SwaggerFilter(IStringLocalizer localizer)
    {
        this._localizer = localizer;
    }

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        DisplayNameAsDescription(swaggerDoc);
        var removePaths = new List<string>();
        foreach (var path in swaggerDoc.Paths)
        {
            //if(path.Operations.First().Value.)
            foreach (var operation in path.Value.Operations)
            {
                if (operation.Value.Tags.Any(tag => tag.Name == "remove"))
                {
                    removePaths.Add(path.Key);
                }
            }
        }
        removePaths.ForEach(key => swaggerDoc.Paths.Remove(key));
    }

    private void DisplayNameAsDescription(OpenApiDocument swaggerDoc)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(o => o.FullName!.StartsWith("WTA"));
        foreach (var item in swaggerDoc.Components.Schemas.Where(o => o.Key.StartsWith("WTA")))
        {
            var type = assemblies.SelectMany(o => o.GetTypes()).FirstOrDefault(o => o.FullName == item.Key);
            if (item.Value.Description == null)
            {
                item.Value.Description = type?.GetCustomAttribute<DisplayAttribute>()?.Name;
            }
            PropertyInfo? pi = null;
            foreach (var propertyItem in item.Value.Properties)
            {
                if (propertyItem.Value.Description == null)
                {
                    if (type != null)
                    {
                        pi = type?.GetProperties().FirstOrDefault(o => o.Name.Equals(propertyItem.Key, StringComparison.OrdinalIgnoreCase));
                        if (pi != null)
                        {
                            var display = pi?.GetCustomAttribute<DisplayAttribute>();
                            if (display != null)
                            {
                                propertyItem.Value.Description = this._localizer.GetString(display.Name!);
                            }
                        }
                    }
                }
            }
        }
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // ?????????????????????????????? Controller ?????????????????????????????????????????? TagsAttribute
        // ??????????????????ControllerName ?????? Entity Name???Entity ??????????????????????????????????????? Action ??????????????????????????? tag ????????????
        if (context.ApiDescription.ActionDescriptor is ControllerActionDescriptor descriptor)
        {
            var controllerType = descriptor.ControllerTypeInfo.AsType();
            var tags = controllerType.GetCustomAttribute<TagsAttribute>()?.Tags;
            if (tags != null)
            {
                if (descriptor.MethodInfo.GetCustomAttributes().FirstOrDefault(o => o.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) is HttpMethodAttribute http
                    && !tags.Any(o => o.Equals($"{http.HttpMethods.FirstOrDefault()}:{descriptor.ActionName}", StringComparison.OrdinalIgnoreCase)))
                {
                    operation.Tags.Add(new OpenApiTag() { Name = "remove" });
                }
            }
        }
    }
}
