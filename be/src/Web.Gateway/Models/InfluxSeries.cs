using System.Text.Json.Serialization;

namespace Web.Gateway.Models;

public class InfluxSeries
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("columns")]
    public List<string> Columns { get; set; }=new List<string>();
    [JsonPropertyName("values")]
    public List<List<string>> Values { get; set; } = new List<List<string>>();
}
