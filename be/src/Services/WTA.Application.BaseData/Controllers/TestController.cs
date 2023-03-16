using Microsoft.AspNetCore.Mvc;
using WTA.Application.Identity.Controllers.Account;

namespace WTA.Application.BaseData.Controllers;

public class TestController : BaseController
{
    public TestController(AccountController accountController)
    {
    }

    [HttpGet]
    public IActionResult Index()
    {
        return Content("Index");
    }
}
