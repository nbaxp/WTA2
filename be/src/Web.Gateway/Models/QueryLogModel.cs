namespace Web.Gateway.Models;

public class QueryLogModel
{
    public string? ApplicationName { get; set; } = string.Empty;
    public bool EnalbeTail { get; set; }
    public List<Dictionary<string, string>> Items { get; set; } = new List<Dictionary<string, string>>();
    public string? Level { get; set; } = string.Empty;
    public List<string> Levels { get; set; } = new List<string>() { "Verbose", "Debug", "Information", "Warning", "Error", "Fatal" };
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;
    public string? Query { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public bool UseCustom { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
}
