using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Gateway.Controllers;

[Authorize]
public class ConfigController : Controller
{
    public IActionResult Index()
    {
        var files = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "settings"), "*.json")
            .Select(o => new { File = o, Content = System.IO.File.ReadAllText(o) })
            .ToDictionary(o => o.File, o => o.Content);
        return View(files);
    }

    [HttpGet]
    public IActionResult Edit(string file)
    {
        return View(model:file);
    }

    [HttpPost]
    public IActionResult Edit(string file, string content)
    {
        using var sw = new StreamWriter(file);
        sw.Write(content);
        return RedirectToAction("Index");
    }
}
