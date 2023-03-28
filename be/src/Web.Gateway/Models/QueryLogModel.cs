using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Gateway.Models;

public class QueryLogModel
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 100;

    public List<SelectListItem> PageSizeList { get; set; } = new List<SelectListItem> {
        new SelectListItem("100", "100"),
        new SelectListItem("1000", "1000"),
        new SelectListItem("10000", "10000") };

    public string? Query { get; set; }
    public string? ApplicationName { get; set; }
    public int Days { get; set; } = 1;
    public bool EnalbeTail { get; set; }
    public InfluxResults InfluxResults { get; set; } = null!;
}
