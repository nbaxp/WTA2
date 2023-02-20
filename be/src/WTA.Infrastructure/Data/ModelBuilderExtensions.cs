using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WTA.Application.Extensions;
using WTA.Core.Application;
using WTA.Core.Domain;
using WTA.Core.Extensions;

namespace WTA.Infrastructure.Data;

public static class ModelBuilderExtensions
{
    public static ModelBuilder ConfigTable(this ModelBuilder builder)
    {
        foreach (var item in builder.Model.GetEntityTypes().Where(o => o.ClrType.IsAssignableTo(typeof(BaseEntity))).ToList())
        {
            builder.Entity(item.Name, o =>
            {
                var prefix = item.ClrType.Assembly.GetCustomAttribute<ModuleAttribute>()?.Name ?? "";
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefix = $"{prefix}_";
                }
                var tableName = $"{prefix}{item.ClrType.Name}";
                o.ToTable(tableName);
                o.ToTable(t => t.HasComment(item.ClrType.GetDisplayName()));
                //
                foreach (var prop in item.GetProperties())
                {
                    if (prop.PropertyInfo != null)
                    {
                        o.Property(prop.Name).HasComment(prop.PropertyInfo.GetDisplayName());
                    }
                }
            });
        }
        return builder;
    }

    public static ModelBuilder ConfigKey(this ModelBuilder builder)
    {
        var propertyName = nameof(BaseEntity.Id);

        foreach (var item in builder.Model.GetEntityTypes().Where(o => o.ClrType.IsAssignableTo(typeof(BaseEntity))).ToList())
        {
            builder.Entity(item.Name).HasKey(propertyName);
            builder.Entity(item.Name).Property(propertyName).ValueGeneratedNever();
        }
        return builder;
    }

    public static ModelBuilder ConfigConcurrencyStamp(this ModelBuilder builder)
    {
        var propertyName = nameof(IConcurrencyStamp.ConcurrencyStamp);
        foreach (var item in builder.Model.GetEntityTypes().Where(o => o.ClrType.IsAssignableTo(typeof(IConcurrencyStamp))).ToList())
        {
            builder.Entity(item.Name).Property(propertyName).IsConcurrencyToken().ValueGeneratedNever();
        }
        return builder;
    }

    public static ModelBuilder ConfigTreeNode(this ModelBuilder builder)
    {
        foreach (var item in builder.Model.GetEntityTypes().Where(o => o.ClrType.IsAssignableTo(typeof(TreeEntity<>))).ToList())
        {
            builder.Entity(item.ClrType).HasOne(nameof(TreeEntity<BaseEntity>.Parent))
                .WithMany(nameof(TreeEntity<BaseEntity>.Children))
                .HasForeignKey(new string[] { nameof(TreeEntity<BaseEntity>.ParentId) }).OnDelete(DeleteBehavior.SetNull);
            builder.Entity(item.ClrType).Property(nameof(TreeEntity<BaseEntity>.Name)).IsRequired();
            builder.Entity(item.ClrType).Property(nameof(TreeEntity<BaseEntity>.Number)).IsRequired();
            // builder.Entity(item.ClrType).Property(nameof(TreeEntity<BaseEntity>.Path)).IsRequired();
        }
        return builder;
    }

    public static ModelBuilder ConfigTenant(this ModelBuilder builder, string? tenant)
    {
        foreach (var entity in builder.Model.GetEntityTypes().Where(o => o.ClrType.IsAssignableTo(typeof(ITenant))).ToList())
        {
            var tenantEntity = entity as ITenant;
            var tenantProperty = entity.FindProperty("Tenant");
            var parameter = Expression.Parameter(entity.ClrType, "p");
            var left = Expression.Property(parameter, tenantProperty!.PropertyInfo!);
            Expression<Func<string>> tenantExpression = () => tenant!;
            var right = tenantExpression.Body;
            var filter = Expression.Lambda(Expression.Equal(left, right), parameter);
            builder.Entity(entity.ClrType).HasQueryFilter(filter);
        }
        return builder;
    }
}
