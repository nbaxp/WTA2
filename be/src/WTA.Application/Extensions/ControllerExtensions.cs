using Microsoft.AspNetCore.Mvc;

namespace WTA.Application.Extensions;

public static class ControllerExtensions
{
    public static IActionResult Result(this Controller controller, object? model, string? viewName = null)
    {
        if (controller.Request.IsJsonRequest())
        {
            return controller.Json(new
            {
                model,
                schema = model?.GetType().GetMetadataForType(controller.HttpContext.RequestServices)
            });
        }
        return viewName == null ? controller.View(model) : controller.View(viewName, model);
    }
}
