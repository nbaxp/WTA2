using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Identity.Domain;
using WTA.Core;
using WTA.Core.Abstractions;

namespace WTA.Application.Identity;

public class IdentityDbContext : IDbContext
{
    public void Initialize(DbContext dbContext)
    {
        var userName = "admin";
        using var scope = App.Services!.CreateScope();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var salt = passwordHasher.CreateSalt();
        var password = "123456";
        dbContext.Set<User>().Add(new User
        {
            UserName = userName,
            NormalizedUserName = userName.Normalize(),
            SecurityStamp = salt,
            PasswordHash = passwordHasher.HashPassword(password, salt),
        });
        dbContext.SaveChanges();
    }

    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(o => o.Id);
    }
}
