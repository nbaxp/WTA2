using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions;
using WTA.Infrastructure.Extensions;

namespace WTA.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IMonitorService _monitorService;

    public HomeController(ILogger<HomeController> logger, IMonitorService monitorService)
    {
        this._logger = logger;
        this._monitorService = monitorService;
    }

    public IActionResult Index()
    {
        this._logger.LogInformation(DateTime.Now.ToString());
        return this.Result(_monitorService.GetStatus());
    }
}
