using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WTA.Application.Identity.Domain.SystemManagement;

namespace WTA.Application.Identity.Data;

public class DefaultConfiguration :
    IEntityTypeConfiguration<Department>,
    IEntityTypeConfiguration<User>,
    IEntityTypeConfiguration<Role>,
    IEntityTypeConfiguration<UserRole>,
    IEntityTypeConfiguration<Permission>,
    IEntityTypeConfiguration<RolePermission>,
    IEntityTypeConfiguration<MenuItem>

{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(o => o.UserName).IsRequired().HasMaxLength(64);
        builder.HasIndex(o => o.UserName);
        // 自动加载
        // builder.Navigation(o => o.UserRoles).AutoInclude();
    }

    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(o => o.Name).IsRequired().HasMaxLength(64);
        builder.HasIndex(o => o.Name);
    }

    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasIndex(o => new { o.UserId, o.RoleId }).IsUnique();
        // 自动加载
        // builder.Navigation(o => o.Role).AutoInclude();
    }

    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(o => o.Name).IsRequired().HasMaxLength(64);
        builder.HasIndex(o => o.Name);
    }

    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.HasIndex(o => new { o.RoleId, o.PermissionId }).IsUnique();
    }

    public void Configure(EntityTypeBuilder<Department> builder)
    {
    }

    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
    }
}
