using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions;
using WTA.Infrastructure.Extensions;

namespace WTA.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IMonitorService _monitorService;

    public HomeController(IMonitorService monitorService)
    {
        this._monitorService = monitorService;
    }

    public IActionResult Index()
    {
        return this.Result(_monitorService.GetStatus());
    }
}
