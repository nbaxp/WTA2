using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WTA.Application.Abstractions;
using WTA.Application.Identity.Domain;
using WTA.Core.Abstractions;

namespace WTA.Application.Identity.Data;

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
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role=new Role{
                        Name= "admin",
                        Nummber="admin"
                    }
                }
            }
        });
        dbContext.SaveChanges();
        var department = new Department
        {
            Name = "1",
            Number = "1",
            Children = new List<Department> {
                new Department
                {
                     Name="1.1",
                     Number="1.1",
                     Children = new List<Department>
                     {
                         new Department
                        {
                             Name="1.1.1",
                             Number="1.1.1",
                        }
                     }
                },
                new Department
                {
                     Name="1.2",
                     Number="1.2",
                }
            }
        };
        department.UpdatePath(null);
        dbContext.Set<Department>().Add(department);
        dbContext.SaveChanges();
    }

    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
