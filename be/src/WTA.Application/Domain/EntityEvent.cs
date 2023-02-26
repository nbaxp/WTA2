using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Domain;

[Display(Name = "实体变动")]
public class EntityEvent : BaseEntity
{
    public DateTimeOffset Date { get; set; }
    public string Entity { get; set; } = null!;
    public string EventType { get; set; } = null!;
    public string? Original { get; set; }
    public string? Current { get; set; } = null!;
}
