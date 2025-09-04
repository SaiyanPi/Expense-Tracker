using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Date)
            .IsRequired();

        builder.Property(e => e.CategoryId)
            .IsRequired();

        builder.Property(e => e.UserId)
           .HasMaxLength(450);

        builder.HasOne(e => e.Category)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne<ApplicationUser>()   // no navigation property
            .WithMany(u => u.Expenses)                
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
