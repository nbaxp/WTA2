using Microsoft.EntityFrameworkCore;

namespace WTA.Application;

public interface IDbContext
{
    void Initialize(DbContext dbContext);

    void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

    void OnModelCreating(ModelBuilder modelBuilder);
}
