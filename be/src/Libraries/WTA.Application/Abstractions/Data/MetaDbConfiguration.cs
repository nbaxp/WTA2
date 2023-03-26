using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WTA.Application.Domain;

namespace WTA.Application.Abstractions.Data;

[DbContext<MetaDbContext>]
public class MetaDbConfiguration : IEntityTypeConfiguration<DbContextHistory>
{
    public void Configure(EntityTypeBuilder<DbContextHistory> builder)
    {
    }
}
