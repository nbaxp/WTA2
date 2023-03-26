using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WTA.Application.Abstractions.Data;

public class MetaDbContext : BaseDbContext<MetaDbContext>, IDbSeed
{
    public MetaDbContext(DbContextOptions<MetaDbContext> options, IServiceScopeFactory serviceProvider) : base(options, serviceProvider)
    {
    }

    public void Seed(DbContext dbContext)
    {
    }
}
