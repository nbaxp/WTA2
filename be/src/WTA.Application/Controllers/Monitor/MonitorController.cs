using System.Globalization;
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
            var fileName = "temp.txt";
            var counter = 0;
            while (true)
            {
                Thread.Sleep(10);
                if (counter > 10000)
                {
                    System.IO.File.WriteAllText(fileName, DateTime.Now.ToString(CultureInfo.InvariantCulture));
                }
                Console.WriteLine(System.IO.File.ReadAllText(fileName));
                using var sw = System.IO.File.AppendText(fileName);
                sw.WriteLine(Guid.NewGuid());
            }
        });
        return Content("Test");
    }
}
