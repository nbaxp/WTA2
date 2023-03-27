using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Embedded;
using Web.Gateway.Models;

namespace Web.Gateway;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index(QueryLogModel model)
    {
        this._logger.LogInformation("test log");
        using var store = EmbeddedServer.Instance.GetDocumentStore("log");
        using var session = store.OpenSession();
        var query = session.Query<LogModel>();
        if (!string.IsNullOrWhiteSpace(model.Query))
        {
            var queryString = model.Query.Trim();
            query = query.Where(o => o.MessageTemplate != null && o.MessageTemplate.Contains(queryString), false);
        }
        model.Total = query.Count();
        model.Items = query.OrderByDescending(o => o.Timestamp)
            .Skip((model.PageIndex - 1) * model.PageSize)
            .Take(model.PageSize)
            .ToList();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Index()
    {
        if (!Request.Body.CanSeek)
        {
            Request.EnableBuffering();
        }

        Request.Body.Position = 0;
        var reader = new StreamReader(Request.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync().ConfigureAwait(false);
        Request.Body.Position = 0;

        using var store = EmbeddedServer.Instance.GetDocumentStore("log");
        using var session = store.OpenSession();
        var list = JsonSerializer.Deserialize<List<LogModel>>(body!);
        list?.ForEach(session.Store);
        session.SaveChanges();

        return Ok();
    }
}
