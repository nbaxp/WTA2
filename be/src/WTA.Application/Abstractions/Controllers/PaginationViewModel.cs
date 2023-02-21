namespace WTA.Application.Abstractions.Controllers;

public class PaginationViewModel<T>
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Query { get; set; }
    public string? OrderBy { get; set; }
    public int TotalCount { get; set; }
    public List<T> Items { get; set; } = new List<T>();
}