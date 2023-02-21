using Microsoft.EntityFrameworkCore;
using WTA.Application.Abstractions;

namespace WTA.Application.BaseData.Data;

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
    }
}
