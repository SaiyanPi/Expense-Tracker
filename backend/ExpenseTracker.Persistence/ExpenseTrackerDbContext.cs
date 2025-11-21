using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence;

public class ExpenseTrackerDbContext : IdentityDbContext<ApplicationUser>
{
    public ExpenseTrackerDbContext(DbContextOptions<ExpenseTrackerDbContext> options)
    : base(options) {}

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpenseTrackerDbContext).Assembly);

        // --- GLOBAL FIX: disable cascade on delete for all FKs ---
        // foreach (var relationship in modelBuilder.Model.GetEntityTypes()
        //     .SelectMany(e => e.GetForeignKeys()))
        // {
        //     relationship.DeleteBehavior = DeleteBehavior.NoAction;
        // }
    }
}
