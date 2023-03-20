using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.Data;

namespace WTA.Application.BaseData.Data;

public class BaseDataDbContext : BaseDbContext<BaseDataDbContext>, IDbSeed
{
    public BaseDataDbContext(DbContextOptions<BaseDataDbContext> options, IServiceScopeFactory serviceProvider) : base(options, serviceProvider)
    {
    }

    public void Seed(DbContext dbContext)
    {
        throw new NotImplementedException();
    }
}
