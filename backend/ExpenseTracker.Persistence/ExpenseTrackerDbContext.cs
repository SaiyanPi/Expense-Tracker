using ExpenseTracker.Application.Common.Auditing.Masking;
using ExpenseTracker.Application.Common.Context;
using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.SharedKernel;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence;

public class ExpenseTrackerDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IUserAccessor? _userAccessor;
    private readonly IRequestContext _requestContext;

    public ExpenseTrackerDbContext(
        DbContextOptions<ExpenseTrackerDbContext> options,
        IRequestContext requestContext,
        IUserAccessor? userAccessor = null
        )
    : base(options)
    {
        _userAccessor = userAccessor;
        _requestContext = requestContext;
    }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpenseTrackerDbContext).Assembly);

        // SELECT * FROM Entity WHERE IsDeleted = 0
        modelBuilder.Entity<Expense>()
            .HasQueryFilter(e => !e.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Budget>()
            .HasQueryFilter(b => !b.IsDeleted);

        // creating indexes in AuditLog
        modelBuilder.Entity<AuditLog>(entity =>
        {
            // Fast ordering + retention cleanup
            entity.HasIndex(a => a.CreatedAt);

            // Admin filtering by entity
            entity.HasIndex(a => new { a.EntityName, a.CreatedAt });

            // User-based audit lookup
            entity.HasIndex(a => new { a.UserId, a.CreatedAt });

            // Activity timeline per entity
            entity.HasIndex(a => new { a.EntityName, a.EntityId, a.CreatedAt });
        });
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userAccessor?.UserId;
        var auditLogs = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries<BaseSoftDeletableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow.ToLocalTime();
                entry.Entity.CreatedBy = userId;

                // auditLogs.Add(new AuditLog
                // {
                //     EntityName = entry.Entity.GetType().Name,
                //     EntityId = entry.Entity.Id.ToString(),
                //     Action = AuditAction.Created,   // 1
                //     NewValues = SerializeScalars(entry.CurrentValues.ToObject()),
                //     UserId = userId,
                //     CreatedAt = DateTime.UtcNow
                // });

                auditLogs.Add(AuditLogFactory.Create(
                    entityName: entry.Entity.GetType().Name,
                    entityId: entry.Entity.Id.ToString(),
                    action: AuditAction.Created,
                    oldValues: null,
                    newValues: SerializeScalars(entry.CurrentValues.ToObject()),
                    userId: userId,
                   
                    requestContext: _requestContext
                ));
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = userId;

                auditLogs.Add(AuditLogFactory.Create(
                    entityName: entry.Entity.GetType().Name,
                    entityId: entry.Entity.Id.ToString(),
                    action: AuditAction.Updated,
                    oldValues: SerializeScalars(entry.OriginalValues.ToObject()),
                    newValues: SerializeScalars(entry.CurrentValues.ToObject()),
                    userId: userId,

                    requestContext: _requestContext
                ));

            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = userId;

                auditLogs.Add(AuditLogFactory.Create(
                    entityName: entry.Entity.GetType().Name,
                    entityId: entry.Entity.Id.ToString(),
                    action: AuditAction.Deleted,
                    oldValues: SerializeScalars(entry.OriginalValues.ToObject()),
                    newValues: null,
                    userId: userId,

                    requestContext: _requestContext
                ));
            }
        }

        if (auditLogs.Any())
        {
            AuditLogs.AddRange(auditLogs);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
    

    // helper
    private static string SerializeScalars(object obj)
    {
        if (obj == null) return "{}";

        var scalarProps = obj.GetType()
            .GetProperties()
            .Where(p => IsScalarType(p.PropertyType)) // only scalar types
            .ToDictionary(p => p.Name, p => p.GetValue(obj));

        return System.Text.Json.JsonSerializer.Serialize(scalarProps);
    }

    // helper to detect scalar types
    private static bool IsScalarType(Type type)
    {
        return
            type.IsPrimitive ||               // int, bool, double...
            type == typeof(string) ||
            type == typeof(decimal) ||
            type == typeof(Guid) ||
            type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) ||
            type == typeof(bool) ||
            type.IsEnum ||
            (Nullable.GetUnderlyingType(type) != null && IsScalarType(Nullable.GetUnderlyingType(type)!));
    }

}