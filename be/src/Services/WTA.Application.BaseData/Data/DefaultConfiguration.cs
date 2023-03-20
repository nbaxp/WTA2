using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WTA.Application.Abstractions.Data;
using WTA.Application.BaseData.Data;
using WTA.Application.BaseData.Domain;

namespace WTA.Application.Identity.Data;

[DbContext<BaseDataDbContext>]
public class DefaultConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
    }
}
