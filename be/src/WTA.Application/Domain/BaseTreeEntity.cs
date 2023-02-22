using Microsoft.AspNetCore.WebUtilities;

namespace WTA.Application.Domain;

public abstract class BaseTreeEntity<T> : BaseEntity where T : class
{
    public List<T> Children { get; set; } = new List<T>();
    public string Name { get; set; } = null!;
    public string Number { get; set; } = null!;
    public T? Parent { get; set; }
    public Guid? ParentId { get; set; }
    public string Path { get; set; } = null!;

    public void UpdatePath(BaseTreeEntity<T>? parent)
    {
        this.Path = $"/{WebEncoders.Base64UrlEncode(this.Id.ToByteArray())}";
        if (parent != null)
        {
            this.Path = $"{parent.Path}{this.Path}";
        }
        if (this.Children.Any())
        {
            this.Children.ForEach(o => (o as BaseTreeEntity<T>)!.UpdatePath(this));
        }
    }
}
