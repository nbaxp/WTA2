using Microsoft.AspNetCore.Mvc;

namespace WTA.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return Content("Index");
    }
}
