using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;

namespace WTA.Application.Domain;

[Query, Create, Update, Delete, Import, Export]
public abstract class BaseEntity : IResource
{
    public BaseEntity()
    {
        this.Id = App.Services!.CreateScope().ServiceProvider.GetService<IGuidGenerator>()!.Create();
    }

    [Display]
    public bool IsReadonly { get; set; }

    [HiddenInput]
    public string? ConcurrencyStamp { get; set; }

    [HiddenInput]
    [Display]
    public DateTimeOffset CreatedAt { get; set; }

    [HiddenInput]
    [Display]
    public string? CreatedBy { get; set; }

    [Display(Order = 100)]
    public bool Disabled { get; set; }

    [Display(Order = -1)]
    public int DisplayOrder { get; set; }

    [HiddenInput]
    public Guid Id { get; set; }

    [HiddenInput]
    public bool IsDeleted { get; set; }

    [HiddenInput]
    public string? DeletedBy { get; set; }

    [HiddenInput]
    [Display]
    public DateTimeOffset? ModifiedAt { get; set; }

    [HiddenInput]
    [Display]
    public string? ModifiedBy { get; set; }

    [HiddenInput]
    [Display]
    public string? Tenant { get; set; }
}
