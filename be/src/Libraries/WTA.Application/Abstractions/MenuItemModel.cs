using WTA.Application.Application;

namespace WTA.Application.Abstractions;

public class MenuItemModel : BaseTreeModel<MenuItemModel>
{
    public string Name { get; set; } = null!;
    public string Number { get; set; } = null!;
    public string? Component { get; set; }
    public string? Redirect { get; set; }
    public string? Icon { get; set; }
    public string? Path { get; set; }
    public string? Url { get; set; }
    public int DisplayOrder { get; set; }
    public string Type { get; set; } = null!;
}
