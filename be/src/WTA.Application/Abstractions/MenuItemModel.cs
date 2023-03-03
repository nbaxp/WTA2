namespace WTA.Application.Abstractions;

public class MenuItemModel
{
    public List<MenuItemModel> Children { get; set; } = new List<MenuItemModel>();
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string Number { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Url { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public Guid? ParentId { get; set; }
}
