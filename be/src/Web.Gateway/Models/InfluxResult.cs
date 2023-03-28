using System.Text.Json.Serialization;

namespace Web.Gateway.Models;

public class InfluxResult
{
    [JsonPropertyName("statement_id")]
    public int StatementId { get; set; }
    [JsonPropertyName("series")]
    public List<InfluxSeries> Series { get; set; } = new List<InfluxSeries>();
}
