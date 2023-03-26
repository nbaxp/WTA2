using Microsoft.AspNetCore.Mvc;

namespace WTA.Application.BaseData.Controllers;

public class TestController : BaseController
{
    public TestController()
    {
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Content("Index");
    }
}
