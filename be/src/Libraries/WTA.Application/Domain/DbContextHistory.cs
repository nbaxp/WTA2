using System.ComponentModel.DataAnnotations;
using WTA.Application.Abstractions.Data;

namespace WTA.Application.Domain;

[Display(Name = "DbContext")]
[DbContext<MetaDbContext>]
public class DbContextHistory : BaseEntity
{
    public string Provider { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Hash { get; set; } = null!;
    public string Sql { get; set; } = null!;
}
