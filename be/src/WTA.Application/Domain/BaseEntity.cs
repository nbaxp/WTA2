using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using WTA.Core.Abstractions;

namespace WTA.Application.Domain;

public abstract class BaseEntity
{
    public BaseEntity()
    {
        this.Id = App.Services!.CreateScope().ServiceProvider.GetService<IGuidGenerator>()!.Create();
    }

    public static readonly ResourceAction List = ResourceAction.List();

    public static readonly ResourceAction Create = ResourceAction.Create();

    public static readonly ResourceAction Update = ResourceAction.Update();

    public static readonly ResourceAction Delete = ResourceAction.Delete();

    [ScaffoldColumn(false)]
    public string? ConcurrencyStamp { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public bool Disabled { get; set; }
    public int DisplayOrder { get; set; }
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsDeletedBy { get; set; }
    public DateTimeOffset? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }

    [ScaffoldColumn(false)]
    public string? Tenant { get; set; }
}
