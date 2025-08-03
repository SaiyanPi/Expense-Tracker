using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Persistence;

public class ExpenseDbContext : IdentityDbContext<ApplicationUser>
{
    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) {}

    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpenseDbContext).Assembly);
    }
}
