using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.Data;
using WTA.Application.Application;
using WTA.Application.Domain;
using WTA.Application.Extensions;
using WTA.Application.Identity.Domain.SystemManagement;

namespace WTA.Application.Identity.Data;

public class IdentityDbContext : BaseDbContext<IdentityDbContext>, IDbSeed
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IServiceScopeFactory serviceScopeFactory) : base(options, serviceScopeFactory)
    {
    }

    public void Seed(DbContext dbContext)
    {
        using var scope = App.Services!.CreateScope();
        var localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer>();
        //
        var homeMenu = new Permission
        {
            Type = PermissionType.Resource,
            Name = "首页",
            Number = "home",
            Path = "/",
            Redirect = "home",
            Icon = "home",
            DisplayOrder = -100
        };
        homeMenu.UpdatePath();
        dbContext.Set<Permission>().Add(homeMenu);
        App.Assemblies
            .Where(o => o.GetCustomAttributes<ModuleAttribute>().Any())
            .ForEach(module =>
            {
                var moduleAttribute = module.GetCustomAttribute<ModuleAttribute>();
                var moduleName = moduleAttribute!.Name;
                var rootMenu = new Permission
                {
                    Type = PermissionType.Module,
                    Number = moduleName,
                    Name = localizer[moduleName],
                    Icon = moduleAttribute.Icon,
                    DisplayOrder = moduleAttribute.Order,
                    Path = $"/{moduleName.ToSlugify()}",
                    Url = $"/{moduleName.ToSlugify()}/"
                };
                //
                module.GetTypes()
                .Where(o => !o.IsAbstract && o.IsAssignableTo(typeof(IResource)) && !o.IsAssignableTo(typeof(IAssociation)))
                .ForEach(resourceType =>
                {
                    var columns = resourceType.GetProperties()
                    .Where(o => o.PropertyType.IsValueType || o.PropertyType == typeof(string))
                    .OrderBy(o => o.GetCustomAttribute<DisplayAttribute>()?.GetOrder())
                    .Select(o => new { PropertyName = o.Name, DisplayName = o.GetDisplayName() })
                    .ToArray();
                    var displayOrder = resourceType.GetCustomAttribute<DisplayAttribute>()?.GetOrder() ?? 0;
                    var resourceMenu = new Permission
                    {
                        Type = PermissionType.Resource,
                        Number = $"{rootMenu.Number}.{resourceType.Name}",
                        Name = resourceType.GetDisplayName(),
                        DisplayOrder = displayOrder,
                        Path = $"{resourceType.Name.ToSlugify()}",
                        Redirect = "index",
                        Url = $"/{moduleName.ToSlugify()}/{resourceType.Name.ToSlugify()}/index",
                        Columns = JsonSerializer.Serialize(columns)
                    };
                    //
                    if (resourceType.GetCustomAttributes().FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(IGroupAttribute))) is IGroupAttribute groupAttribute)
                    {
                        var groupMenuNumber = $"{rootMenu.Number}.{groupAttribute.Name}";
                        var groupMenu = rootMenu.Children.FirstOrDefault(o => o.Number == groupMenuNumber);
                        if (groupMenu == null)
                        {
                            groupMenu = new Permission
                            {
                                Type = PermissionType.Resource,
                                Number = groupMenuNumber,
                                Name = localizer[groupAttribute.Name],
                                Icon = groupAttribute.Icon,
                                DisplayOrder = groupAttribute.DisplayOrder,
                                Path = $"{groupAttribute.Name.ToSlugify()}",
                                Url = $"/{moduleName.ToSlugify()}/{groupAttribute.Name.ToSlugify()}/"
                            };
                            rootMenu.Children.Add(groupMenu);
                        }
                        groupMenu.Children.Add(resourceMenu);
                        resourceMenu.Number = $"{groupMenu.Number}.{resourceType.Name}";
                        resourceMenu.Url = $"{groupMenu.Path}{resourceType.Name.ToSlugify()}/index";
                    }
                    else
                    {
                        rootMenu.Children.Add(resourceMenu);
                    }
                    resourceType.GetCustomAttributes(true).ForEach(attribute =>
                    {
                        if (attribute is BaseActionAttribute actionAttribute)
                        {
                            var actionMenu = new Permission
                            {
                                Type = PermissionType.Action,
                                Number = $"{resourceMenu.Number}.{actionAttribute.Name}",
                                Name = localizer[actionAttribute.Name],
                                Path = $"{actionAttribute.Name.ToSlugify()}",
                                Url = $"{resourceMenu.Url}/{actionAttribute.Name.ToSlugify()}",
                                Icon = actionAttribute.Icon,
                                DisplayOrder = actionAttribute.DisplayOrder
                            };
                            resourceMenu.Children.Add(actionMenu);
                        }
                    });
                });
                //
                rootMenu.UpdatePath();
                dbContext.Set<Permission>().Add(rootMenu);
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
