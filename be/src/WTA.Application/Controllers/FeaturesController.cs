using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace WTA.Application.Services;

[Route("[controller]/[action]")]
public class FeaturesController : Controller
{
    private readonly ApplicationPartManager _partManager;

    public FeaturesController(ApplicationPartManager partManager)
    {
        _partManager = partManager;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var controllerFeature = new ControllerFeature();
        _partManager.PopulateFeature(controllerFeature);

        var tagHelperFeature = new TagHelperFeature();
        _partManager.PopulateFeature(tagHelperFeature);

        var viewComponentFeature = new ViewComponentFeature();
        _partManager.PopulateFeature(viewComponentFeature);

        return Json(new
        {
            Controllers = controllerFeature.Controllers.Select(o => o.Name + string.Join(',', o.GenericTypeArguments?.Select(o => o.Name)!)).ToList(),
            TagHelpers = tagHelperFeature.TagHelpers.Select(o => o.Name).ToList(),
            ViewComponents = viewComponentFeature.ViewComponents.Select(o => o.Name).ToList()
        });
    }
}
