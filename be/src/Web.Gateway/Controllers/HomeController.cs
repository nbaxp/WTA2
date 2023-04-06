using System.Globalization;
using System.Text;
using System.Text.Json;
using Flurl;
using Flurl.Util;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Web.Gateway.Models;

namespace Web.Gateway;

public class HomeController : Controller
{
    private const string Table = "log";
    private const string TagKey = "ApplicationName";
    private const string TimeKey = "Time";
    private readonly Dictionary<string, string> _connectionValues;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<PageHub> _hubContext;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        IHubContext<PageHub> hubContext)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
        _connectionValues = configuration.GetConnectionString("InfluxDB")
            .Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(o => o.Split('=', StringSplitOptions.RemoveEmptyEntries))
            .ToDictionary(o => o[0].ToLowerInvariant(), o => o[1]);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(QueryLogModel model)
    {
        try
        {
            if (!model.UseCustom || string.IsNullOrEmpty(model.Query))
            {
                if (!model.Start.HasValue)
                {
                    model.Start = DateTime.Now.Date;
                }
                if (!model.End.HasValue)
                {
                    model.End = DateTime.Now.Date.AddDays(1);
                }
                var start = model.Start.Value.ToUniversalTime().ToInvariantString();
                var end = model.End.Value.ToUniversalTime().ToInvariantString();
                var query = $"select * from {Table} where time>='{start}' and time<='{end}' ";
                if (!string.IsNullOrEmpty(model.ApplicationName))
                {
                    query += $"and {nameof(model.ApplicationName)}='{model.ApplicationName}' ";
                }
                else
                {
                    model.ApplicationName = string.Empty;
                }
                if (!string.IsNullOrEmpty(model.Level))
                {
                    query += $"and {nameof(model.Level)}='{model.Level}' ";
                }
                else
                {
                    model.Level = string.Empty;
                }
                query += $"order by time desc ";
                query += $"limit {model.PageSize} offset {(model.PageIndex - 1) * model.PageSize}";
                model.Query = query;
            }
            var result = await QueryInfluxDB($"{model.Query}").ConfigureAwait(false);
            if (result != null)
            {
                model.Items = result.Values.Select(o =>
                {
                    var dict = new Dictionary<string, string>();
                    for (int i = 0; i < result.Columns.Count; i++)
                    {
                        dict.Add(result.Columns[i], o[i]);
                    }
                    return dict;
                }).ToList();
            }
            // tags
            var tagQuery = $"show tag values on {_connectionValues["database"]} with key={TagKey}";
            var tagResult = await QueryInfluxDB(tagQuery).ConfigureAwait(false);
            if (tagResult != null)
            {
                model.Tags = tagResult.Values.Select(o => o[1]).ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.ToString());
            ModelState.AddModelError("", ex.Message);
        }
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
        var list = JsonSerializer.Deserialize<List<LogModel>>(body!)!;

        using var client = new InfluxDBClient(_connectionValues["url"],
            _connectionValues["usr"],
            _connectionValues["pwd"],
            _connectionValues["database"],
            _connectionValues["retention-policy"]);
        using var writeApi = client.GetWriteApi();
        var dicts = list.Select(o =>
        {
            var dict = new Dictionary<string, string>
            {
                { TimeKey,new DateTimeOffset(o.Timestamp).ToInvariantString()},
                { TagKey, o.Properties?[TagKey].ToString()! },
                { nameof(o.Level), o.Level },
            };
            if (o.Exception != null)
            {
                dict.Add(nameof(o.Exception), o.Exception);
            }
            if (o.MessageTemplate != null)
            {
                dict.Add(nameof(o.MessageTemplate), o.MessageTemplate);
            }
            if (o.RenderedMessage != null)
            {
                dict.Add(nameof(o.RenderedMessage), o.RenderedMessage);
            }
            if (o.Properties != null)
            {
                foreach (var item in o.Properties.Where(o => o.Key != TagKey && o.Value != null))
                {
                    if (item.Key == "EventId")
                    {
                        var eventProperties = item.Value as JsonElement?;
                        if (eventProperties != null)
                        {
                            if (eventProperties.Value.TryGetProperty("Id", out JsonElement eventId))
                            {
                                dict.Add("EventId", eventId.ToString());
                            }
                        }
                        if (eventProperties != null)
                        {
                            if (eventProperties.Value.TryGetProperty("Name", out JsonElement eventName))
                            {
                                dict.Add("EventName", eventName.ToString());
                            }
                        }
                    }
                    else
                    {
                        dict.Add(item.Key, item.Value.ToString()!);
                    }
                }
            }
            if (o.Renderings != null)
            {
                foreach (var item in o.Renderings)
                {
                    foreach (var item2 in item.Value)
                    {
                        dict.Add($"{item.Key}_{item2.Format}", item2.Rendering);
                    }
                }
            }
            return dict;
        });
        var points = dicts.Select(o =>
        {
            var point = PointData.Measurement(Table);
            foreach (var key in o.Keys)
            {
                if (key == TimeKey)
                {
                    point = point.Timestamp(DateTime.Parse(o[key], CultureInfo.InvariantCulture), WritePrecision.Ms);
                }
                else if (key == TagKey)
                {
                    point = point.Tag(TagKey, o[key]);
                }
                else
                {
                    point = point.Field(key, o[key]);
                }
            }
            return point;
        }).ToList();
        writeApi.WritePoints(points);
        await _hubContext.Clients.Group("tail").SendAsync("notify", dicts.Reverse()).ConfigureAwait(false);
        return Ok();
    }

    private async Task<InfluxSeries?> QueryInfluxDB(string query)
    {
        var url = _connectionValues["url"]
            .AppendPathSegment("query")
            .SetQueryParam("db", _connectionValues["database"])
            .SetQueryParam("q", query!)
            .ToString();
        var result = await _httpClientFactory.CreateClient().GetAsync(url).ConfigureAwait(false);
        var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
        var results = JsonSerializer.Deserialize<InfluxResults>(content)!;
        if (results.Results.Count > 0 && results.Results[0].Series.Count > 0)
        {
            return results.Results[0].Series[0];
        }
        return null;
    }
}
