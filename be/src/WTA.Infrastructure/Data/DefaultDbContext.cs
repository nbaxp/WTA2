using System.Linq.Expressions;
using System.Reflection;
using LinqToDB.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WTA.Application;
using WTA.Application.Abstractions;
using WTA.Application.Abstractions.EventBus;
using WTA.Application.Application;
using WTA.Application.Domain;
using WTA.Application.Extensions;

namespace WTA.Infrastructure.Data;

public class DefaultDbContext : DbContext
{
    public static readonly ILoggerFactory DefaultLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

    private readonly IServiceScopeFactory _serviceScopeFactory;

    static DefaultDbContext()
    {
        LinqToDBForEFTools.Initialize();
    }

    public DefaultDbContext(DbContextOptions options, IServiceScopeFactory serviceScopeFactory) : base(options)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override int SaveChanges()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = base.SaveChanges();
        AfterSave(entries, services);
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        AfterSave(entries, services);
        return result;
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var services = scope.ServiceProvider;
        var entries = GetEntries();
        BeforeSave(entries, services);
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
        AfterSave(entries, services);
        return result;
    }

    public void Seed()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        App.DbContextList?.ForEach(o => o.OnConfiguring(optionsBuilder));
        optionsBuilder.UseLoggerFactory(DefaultLoggerFactory);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 默认创建 DbContext 的 History 表
        App.DbContextList?.ForEach(o =>
        {
            o.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(o.GetType().Assembly);
        });
        //
        modelBuilder.Entity<DbContextHistory>();
        modelBuilder.Entity<EntityEvent>();
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
            using var scope = _serviceScopeFactory.CreateScope();
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

    private void AfterSave(List<EntityEntry> entries, IServiceProvider services)
    {
        foreach (var entry in entries)
        {
            if (entry.Entity.GetType() != typeof(EntityEvent))
            {
                var events = new List<object>();
                if (entry.State == EntityState.Added)
                {
                    events.Add(Activator.CreateInstance(typeof(EntityCreatedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity)!);
                }
                else if (entry.State == EntityState.Modified)
                {
                    events.Add(Activator.CreateInstance(typeof(EntityUpdatedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity, entry.OriginalValues.ToObject())!);
                }
                else if (entry.State == EntityState.Deleted)
                {
                    events.Add(Activator.CreateInstance(typeof(EntityDeletedEvent<>).MakeGenericType(entry.Entity.GetType()), entry.Entity)!);
                }
                var publisher = services.GetRequiredService<IEventPublisher>();
                events.ForEach(o => publisher.Publish(o));
            }
        }
    }

    private void BeforeSave(List<EntityEntry> entries, IServiceProvider services)
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
                if (item.Entity.GetType() != typeof(EntityEvent))
                {
                    if (item.State == EntityState.Added || item.State == EntityState.Modified || item.State == EntityState.Deleted)
                    {
                        this.Set<EntityEvent>().Add(new EntityEvent
                        {
                            Date = DateTimeOffset.Now,
                            Entity = item.Entity.GetType().Name,
                            EventType = item.State.ToString(),
                            Original = item.State == EntityState.Added ? null : item.OriginalValues.ToObject().ToJson(),
                            Current = item.State == EntityState.Deleted ? null : item.Entity.ToJson(),
                            CreatedAt = DateTimeOffset.Now,
                            CreatedBy = userName,
                        });
                    }
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
