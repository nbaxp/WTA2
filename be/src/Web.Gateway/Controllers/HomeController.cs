using System.Text;
using System.Text.Json;
using Flurl;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Gateway.Models;

namespace Web.Gateway;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<PageHub> _hubContext;

    public HomeController(ILogger<HomeController> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IHubContext<PageHub> hubContext)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<IActionResult> Index(QueryLogModel model)
    {
        this._logger.LogInformation("test log");
        if (string.IsNullOrEmpty(model.Query))
        {
            var query = $"select time,ApplicationName,RenderedMessage from log where time>now() - {model.Days}d ";
            query += $"limit {model.PageSize} offset {(model.PageIndex - 1) * model.PageSize}";
            model.Query = query;
        }
        var connectionValues = this.GetConnectionValues();
        var httpClient = _httpClientFactory.CreateClient();
        var url = connectionValues["url"]
            .AppendPathSegment("query")
            .SetQueryParam("db", connectionValues["database"])
            .SetQueryParam("q", model.Query!)
            .ToString();
        var result = await httpClient.GetAsync(url).ConfigureAwait(false);
        var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        model.InfluxResults = JsonSerializer.Deserialize<InfluxResults>(content)!;
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
        var list = JsonSerializer.Deserialize<List<LogModel>>(body!);

        using var client = CreateClient(this.GetConnectionValues());
        using var writeApi = client.GetWriteApi();
        var table = "log";
        var tagName = "ApplicationName";
        var points = list!.Select(o =>
        {
            var point = PointData.Measurement(table)
            .Timestamp(new DateTimeOffset(o.Timestamp).ToUnixTimeMilliseconds(), WritePrecision.Ms)
            .Tag(tagName, o.Properties?[tagName].ToString())
            .Field(nameof(o.Level), o.Level)
            .Field(nameof(o.MessageTemplate), o.MessageTemplate)
            .Field(nameof(o.RenderedMessage), o.RenderedMessage)
            .Field(nameof(o.Exception), o.Exception);
            if (o.Properties != null)
            {
                foreach (var item in o.Properties.Where(o => o.Key != tagName))
                {
                    point = point.Field(item.Key, item.Value);
                }
            }
            if (o.Renderings != null)
            {
                foreach (var item in o.Renderings)
                {
                    foreach (var item2 in item.Value)
                    {
                        point = point.Field($"{item.Key}_{item2.Format}", item2.Rendering);
                    }
                }
            }
            return point;
        }).ToList();
        writeApi.WritePoints(points);
        await _hubContext.Clients.Group("tail").SendAsync("notify", points).ConfigureAwait(false);
        return Ok();
    }

    private Dictionary<string, string> GetConnectionValues()
    {
        return this._configuration.GetConnectionString("InfluxDB")
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.Split('=', StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(o => o[0].ToLowerInvariant(), o => o[1]);
    }

    private static InfluxDBClient CreateClient(Dictionary<string, string> connectionValues)
    {
        return new InfluxDBClient(connectionValues["url"],
            connectionValues["usr"],
            connectionValues["pwd"],
            connectionValues["database"],
            connectionValues["retention-policy"]);
    }
}
