using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions.Data;

namespace WTA.Application.BaseData.Data;

public class BaseDataDbContext : BaseDbContext<BaseDataDbContext>
{
    public BaseDataDbContext(DbContextOptions<BaseDataDbContext> options, IServiceScopeFactory serviceProvider) : base(options, serviceProvider)
    {
    }
}
