using Microsoft.EntityFrameworkCore;
using WTA.Application.BaseData.Domain;

namespace WTA.Application.BaseData;

public class BaseDataDbContext : IDbContext
{
    public void Initialize(DbContext dbContext)
    {
    }

    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>();
    }
}
