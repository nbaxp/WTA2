using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using WTA.Application.Abstractions;
using WTA.Application.Application;
using WTA.Application.Domain;
using WTA.Application.Extensions;
using WTA.Application.Identity.Domain.SystemManagement;

namespace WTA.Application.Identity.Data;

public class IdentityDbContext : IDbContext
{
    public void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }

    public void OnModelCreating(ModelBuilder modelBuilder)
    {
    }

    public void Seed(DbContext dbContext)
    {
        using var scope = App.Services!.CreateScope();
        var localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer>();
        //
        var homeMenu = new MenuItem
        {
            Name = "首页",
            Number = "home",
            Url = "/",
            Icon = "home",
            DisplayOrder = -100
        };
        homeMenu.UpdatePath();
        dbContext.Set<MenuItem>().Add(homeMenu);
        App.ModuleAssemblies?.ForEach(module =>
        {
            var moduleAttribute = module.GetCustomAttribute<ModuleAttribute>();
            var moduleName = moduleAttribute!.Name;
            var rootMenu = new MenuItem
            {
                Number = moduleName,
                Name = localizer[moduleName],
                Icon = moduleAttribute.Icon,
                DisplayOrder = moduleAttribute.Order,
                Url = $"/{moduleName.ToSlugify()}/"
            };
            //
            module.GetTypes()
            .Where(o => !o.IsAbstract && o.IsAssignableTo(typeof(IResource)) && !o.IsAssignableTo(typeof(IAssociation)))
            .ForEach(o =>
            {
                var displayOrder = o.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? 0;
                var resourceMenu = new MenuItem
                {
                    Number = $"{moduleName}.{o.Name}",
                    Name = o.GetDisplayName(),
                    DisplayOrder = displayOrder,
                    Url = $"/{moduleName.ToSlugify()}/{o.Name.ToSlugify()}/index"
                };
                //
                var groupAttribute = o.GetCustomAttributes().FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(IGroupAttribute))) as IGroupAttribute;
                if (groupAttribute != null)
                {
                    var groupMenu = rootMenu.Children.FirstOrDefault(o => o.Number == groupAttribute.Name);
                    if (groupMenu == null)
                    {
                        groupMenu = new MenuItem
                        {
                            Number = groupAttribute.Name,
                            Name = localizer[groupAttribute.Name],
                            Icon = groupAttribute.Icon,
                            DisplayOrder = groupAttribute.DisplayOrder,
                            Url = $"/{moduleName.ToSlugify()}/{groupAttribute.Name.ToSlugify()}/"
                        };
                        rootMenu.Children.Add(groupMenu);
                    }
                    groupMenu.Children.Add(resourceMenu);
                    resourceMenu.Url = $"{groupMenu.Url}{o.Name.ToSlugify()}/index";
                }
                else
                {
                    rootMenu.Children.Add(resourceMenu);
                }
            });
            //
            rootMenu.UpdatePath();
            dbContext.Set<MenuItem>().Add(rootMenu);
        });
        dbContext.SaveChanges();
        //
        //var resourceTypes = App.ModuleAssemblies?
        //    .SelectMany(o => o.GetTypes())
        //    .Where(o => o.IsAssignableTo(typeof(BaseEntity)))
        //    .ToList();
        //if (resourceTypes != null)
        //{
        //    foreach (var resourceType in resourceTypes)
        //    {
        //        var permission = new Permission
        //        {
        //            Type = PermissionType.Resource,
        //            Number = resourceType.Name,
        //            Name = resourceType.GetDisplayName()
        //        };
        //        var actions = new List<ResourceAction>();
        //        var tempType = resourceType;
        //        while (tempType.BaseType != null)
        //        {
        //            var tempActions = tempType.GetFields(BindingFlags.Public | BindingFlags.Static)
        //                    .Where(o => o.FieldType == typeof(ResourceAction))
        //                    .Select(o => o.GetValue(null) as ResourceAction)
        //                    .ToList();
        //            actions.AddRange(tempActions!);
        //            tempType = tempType.BaseType;
        //        }
        //        foreach (var action in actions)
        //        {
        //            permission.Children.Add(new Permission
        //            {
        //                Type = PermissionType.Permission,
        //                Number = action.Name,
        //                Name = localizer[action.Name]
        //            });
        //        }
        //        permission.UpdatePath(null);
        //        dbContext.Set<Permission>().Add(permission);
        //    }
        //}
        //dbContext.SaveChanges();
        //
        var userName = "admin";
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
                        Nummber="admin",
                        IsReadonly=true,
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
}
