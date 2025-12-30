using ExpenseTracker.Application.Common.Interfaces.Services;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence;

public class ExpenseTrackerDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly IUserAccessor? _userAccessor;
    public ExpenseTrackerDbContext(
        DbContextOptions<ExpenseTrackerDbContext> options,
        IUserAccessor? userAccessor = null)
    : base(options)
    {
        _userAccessor = userAccessor;
    }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // SELECT * FROM Entity WHERE IsDeleted = 0
        modelBuilder.Entity<Expense>()
            .HasQueryFilter(e => !e.IsDeleted);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Budget>()
            .HasQueryFilter(b => !b.IsDeleted);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpenseTrackerDbContext).Assembly);

        // --- GLOBAL FIX: disable cascade on delete for all FKs ---
        // foreach (var relationship in modelBuilder.Model.GetEntityTypes()
        //     .SelectMany(e => e.GetForeignKeys()))
        // {
        //     relationship.DeleteBehavior = DeleteBehavior.NoAction;
        // }
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var userId = _userAccessor?.UserId;
        var auditLogs = new List<AuditLog>();

        foreach (var entry in ChangeTracker.Entries<BaseSoftDeletableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedBy = userId;

                auditLogs.Add(new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = entry.Entity.Id.ToString(),
                    Action = AuditAction.Created,   // 1
                    NewValues = SerializeScalars(entry.CurrentValues.ToObject()),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedBy = userId;

                auditLogs.Add(new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = entry.Entity.Id.ToString(),
                    Action = AuditAction.Updated,   // 2
                    OldValues = SerializeScalars(entry.OriginalValues.ToObject()),
                    NewValues = SerializeScalars(entry.CurrentValues.ToObject()),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsDeleted = true;
                entry.Entity.DeletedAt = DateTime.UtcNow;
                entry.Entity.DeletedBy = userId;

                auditLogs.Add(new AuditLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    EntityId = entry.Entity.Id.ToString(),
                    Action = AuditAction.Deleted,   // 3
                    OldValues = SerializeScalars(entry.OriginalValues.ToObject()),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                });
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