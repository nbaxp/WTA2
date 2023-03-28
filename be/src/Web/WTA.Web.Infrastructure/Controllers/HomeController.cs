using Microsoft.AspNetCore.Mvc;

namespace WTA.Web.Infrastructure.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return Content("Index");
    }
}
