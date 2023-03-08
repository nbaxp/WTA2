using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using WTA.Application.Abstractions;
using WTA.Application.Extensions;

namespace WTA.Application.Services.Monitor;

public class MonitorController : Controller
{
    private readonly IMonitorService _monitorService;

    public MonitorController(IMonitorService monitorService)
    {
        this._monitorService = monitorService;
    }
    public IActionResult Index()
    {
        return this.Result(_monitorService.GetStatus());
    }
}
