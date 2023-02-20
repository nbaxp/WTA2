using Microsoft.Extensions.DependencyInjection;
using WTA.Core.Abstractions;

namespace WTA.Core.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; set; }

    public BaseEntity()
    {
        Id = App.Services!.CreateScope().ServiceProvider.GetService<IGuidGenerator>()!.Create();
    }
}
