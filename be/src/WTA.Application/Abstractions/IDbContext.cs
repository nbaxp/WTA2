using Microsoft.EntityFrameworkCore;

namespace WTA.Application.Abstractions;

public interface IDbContext
{
    void Seed(DbContext dbContext);

    void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

    void OnModelCreating(ModelBuilder modelBuilder);
}
