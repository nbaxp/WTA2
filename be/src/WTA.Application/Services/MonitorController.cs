using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace WTA.Application.Services;

public class MonitorController : Controller
{
    public IActionResult Index()
    {
        var process = Process.GetCurrentProcess();
        return Json(new
        {
            OSArchitecture=RuntimeInformation.OSArchitecture.ToString(),
            RuntimeInformation.OSDescription,
            Environment.Is64BitOperatingSystem,
            ProcessArchitecture =RuntimeInformation.ProcessArchitecture.ToString(),
            Environment.Is64BitProcess,
            WorkingSet64 = process.WorkingSet64 / 1024.0,
            process.TotalProcessorTime.TotalSeconds
        });
    }
}
