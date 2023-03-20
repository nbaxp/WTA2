using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WTA.Application.Application;
using WTA.Application.Domain;
using WTA.Application.Extensions;

namespace WTA.Application.Abstractions.Data;

public abstract class BaseDbContext<T> : DbContext where T : DbContext
{
    public static readonly ILoggerFactory DefaultLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    private readonly IServiceScopeFactory _serviceProvider;

    public BaseDbContext(DbContextOptions<T> options, IServiceScopeFactory serviceProvider) : base(options)
    {
        this._serviceProvider = serviceProvider;
    }

    public override int SaveChanges()
    {
        using var scope = this._serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = base.SaveChanges();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = this._serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        using var scope = this._serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
        return result;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(DefaultLoggerFactory);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var applyEntityConfigurationMethod = typeof(ModelBuilder)
            .GetMethods()
            .Single(
                e => e.Name == nameof(modelBuilder.ApplyConfiguration)
                    && e.ContainsGenericParameters
                    && e.GetParameters().SingleOrDefault()?.ParameterType.GetGenericTypeDefinition()
                    == typeof(IEntityTypeConfiguration<>));
        var list = GetType().Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .Where(t => t.GetCustomAttributes().Any(o => o.GetType().IsGenericType && o.GetType().GetGenericTypeDefinition() == typeof(DbContextAttribute<>))).ToList();
        GetType().Assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .Where(t => t.GetCustomAttributes().Any(o => o.GetType().IsGenericType && o.GetType().GetGenericTypeDefinition() == typeof(DbContextAttribute<>)))
            .ForEach(t =>
            {
                var config = Activator.CreateInstance(t)!;
                t.GetInterfaces().Where(o => o.IsGenericType && o.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                .ForEach(t =>
                {
                    applyEntityConfigurationMethod.MakeGenericMethod(t.GetGenericArguments()[0]).Invoke(modelBuilder, new object[] { config });
                });
            });
        //
        foreach (var item in modelBuilder.Model.GetEntityTypes().Where(o => o.ClrType.IsAssignableTo(typeof(BaseEntity))).ToList())
        {
            // 设置Id
            modelBuilder.Entity(item.ClrType).HasKey(nameof(BaseEntity.Id));
            modelBuilder.Entity(item.ClrType).Property(nameof(BaseEntity.Id)).ValueGeneratedNever();
            // 设置行版本号
            modelBuilder.Entity(item.ClrType).Property(nameof(BaseEntity.ConcurrencyStamp)).ValueGeneratedNever();
            // 配置表名称和注释
            modelBuilder.Entity(item.ClrType, o =>
            {
                var prefix = item.ClrType.Assembly.GetCustomAttribute<ModuleAttribute>()?.Name ?? "";
                if (!string.IsNullOrEmpty(prefix))
                {
                    prefix = $"{prefix}_";
                }
                var tableName = $"{prefix}{item.GetTableName()}";
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
            // 配置 TreeEntity
            if (item.ClrType.IsAssignableTo(typeof(BaseTreeEntity<>).MakeGenericType(item.ClrType)))
            {
                modelBuilder.Entity(item.ClrType).HasOne(nameof(BaseTreeEntity<BaseEntity>.Parent))
                    .WithMany(nameof(BaseTreeEntity<BaseEntity>.Children))
                    .HasForeignKey(new string[] { nameof(BaseTreeEntity<BaseEntity>.ParentId) })
                    .OnDelete(DeleteBehavior.SetNull);
                modelBuilder.Entity(item.ClrType).Property(nameof(BaseTreeEntity<BaseEntity>.Name)).IsRequired();
                modelBuilder.Entity(item.ClrType).Property(nameof(BaseTreeEntity<BaseEntity>.Number)).IsRequired().HasMaxLength(64);
                modelBuilder.Entity(item.ClrType).Property(nameof(BaseTreeEntity<BaseEntity>.InternalPath)).IsRequired();
                modelBuilder.Entity(item.ClrType).HasIndex(nameof(BaseTreeEntity<BaseEntity>.Number)).IsUnique();
            }
            // 配置多租户
            using var scope = this._serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var tenant = services.GetRequiredService<ITenantService>().Tenant;
            var tenantProperty = item.FindProperty("Tenant");
            var parameter = Expression.Parameter(item.ClrType, "p");
            var left = Expression.Property(parameter, tenantProperty!.PropertyInfo!);
            Expression<Func<string>> tenantExpression = () => tenant!;
            var right = tenantExpression.Body;
            var filter = Expression.Lambda(Expression.Equal(left, right), parameter);
            modelBuilder.Entity(item.ClrType).HasQueryFilter(filter);
        }
    }

    protected virtual void BeforeSave(List<EntityEntry> entries, IServiceProvider services)
    {
        var userName = services.GetRequiredService<IHttpContextAccessor>().HttpContext?.User.Identity?.Name;
        var tenant = services.GetRequiredService<ITenantService>().Tenant;
        foreach (var item in entries.Where(o => o.State == EntityState.Added || o.State == EntityState.Modified || o.State == EntityState.Deleted))
        {
            // 设置审计属性和租户
            if (item.Entity is BaseEntity entity)
            {
                if (item.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTimeOffset.Now;
                    entity.CreatedBy = userName;
                    entity.Tenant = tenant;
                }
                else if (item.State == EntityState.Modified)
                {
                    entity.ModifiedAt = DateTimeOffset.Now;
                    entity.ModifiedBy = userName;
                }
                if (item.State != EntityState.Deleted)
                {
                    entity.ConcurrencyStamp = Guid.NewGuid().ToString();
                }
            }
        }
    }

    private List<EntityEntry> GetEntries()
    {
        this.ChangeTracker.DetectChanges();
        var entries = this.ChangeTracker.Entries().ToList();
        return entries;
    }
}
