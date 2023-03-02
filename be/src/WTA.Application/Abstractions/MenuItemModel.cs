namespace WTA.Application.Abstractions;

public class MenuItemModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;

    public string Number { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public List<MenuItemModel> Children { get; set; } = new List<MenuItemModel>();
}
