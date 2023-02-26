using System.ComponentModel.DataAnnotations;

namespace WTA.Application.Domain;

[Display(Name = "数据上下文")]
public class DbContextHistory : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Hash { get; set; } = null!;
}
