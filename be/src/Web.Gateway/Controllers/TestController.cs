using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Web.Gateway.Controllers;

public class TestController : Controller
{
    private readonly IConfiguration _configuration;

    public TestController(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public IActionResult Index()
    {
        return Json(this._configuration.AsEnumerable(),new JsonSerializerOptions { WriteIndented=true });
    }
}
