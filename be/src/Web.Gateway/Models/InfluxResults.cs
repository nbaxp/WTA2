using System.Text.Json.Serialization;

namespace Web.Gateway.Models;

public class InfluxResults
{
    [JsonPropertyName("results")]
    public List<InfluxResult> Results { get; set; } = new List<InfluxResult>();
}
