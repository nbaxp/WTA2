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

    public IActionResult Test()
    {
        Task.Run(() =>
        {
            decimal result = 0;
            Random rnd = new Random();
            while (true)
            {
                result += rnd.Next(0, 100);
                if (result % 100000 == 0)
                {
                    Thread.Sleep(10);//模拟其他非CPU计算操作
                }
            }
        });
        return Content("Test");
    }
}
