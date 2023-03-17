using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace WTA.Application.Domain;

public abstract class BaseTreeEntity<T> : BaseEntity where T : class
{
    public List<T> Children { get; set; } = new List<T>();

    [Display]
    public string Name { get; set; } = null!;

    [Display]
    public string Number { get; set; } = null!;

    public T? Parent { get; set; }

    [HiddenInput]
    public Guid? ParentId { get; set; }

    [HiddenInput]
    public string InternalPath { get; set; } = null!;

    public T UpdatePath(BaseTreeEntity<T>? parent = null)
    {
        this.InternalPath = $"/{WebEncoders.Base64UrlEncode(this.Id.ToByteArray())}";
        if (parent != null)
        {
            this.InternalPath = $"{parent.InternalPath}{this.InternalPath}";
        }
        if (this.Children.Any())
        {
            this.Children.ForEach(o => (o as BaseTreeEntity<T>)!.UpdatePath(this));
        }
        return (this as T)!;
    }
}
