using Microsoft.AspNetCore.Mvc.Rendering;

namespace WTA.LogServer.Models;

public class QueryModel
{
    public List<LogEventModel> Items = new List<LogEventModel>();
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public List<SelectListItem> PageSizeList { get; set; } = new List<SelectListItem> {
        new SelectListItem("10", "10"),
        new SelectListItem("100", "100"),
        new SelectListItem("1000", "1000") };
    public string? Query { get; set; }
    public long Total { get; set; }
}
