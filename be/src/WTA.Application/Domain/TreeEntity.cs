namespace WTA.Application.Domain;

public abstract class TreeEntity<T> : BaseEntity where T : BaseEntity
{
    public List<T> Children { get; set; } = new List<T>();
    public string Name { get; set; } = null!;
    public string Number { get; set; } = null!;
    public int Order { get; set; }

    public T? Parent { get; set; }

    // public string Path { get; set; }
    public Guid? ParentId { get; set; }
}
