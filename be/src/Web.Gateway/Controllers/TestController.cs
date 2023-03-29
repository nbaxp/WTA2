using Microsoft.AspNetCore.Mvc;

namespace Web.Gateway.Controllers;

public class TestController : Controller
{
    private readonly ILogger<TestController> _logger;

    public TestController(ILogger<TestController> logger)
    {
        this._logger = logger;
    }

    public IActionResult Index()
    {
        this._logger.LogInformation("test LogInformation");
        this._logger.LogWarning("test LogWarning");
        this._logger.LogDebug("test LogDebug");
        this._logger.LogError("test LogError");
        this._logger.LogTrace("test LogTrace");
        return Content("Test");
    }
}
