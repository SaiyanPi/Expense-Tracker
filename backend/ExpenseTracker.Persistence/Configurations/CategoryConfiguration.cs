using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne<ApplicationUser>() // reference ApplicationUser type
            .WithMany() // no navigation property in ApplicationUser
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        //📝 following col is commented because i've already defined the category-expense relationship in
        // the expense configuration file since expense is the dependent entity
        // builder.HasMany(c => c.Expenses)
        //     .WithOne(e => e.Category)
        //     .HasForeignKey(e => e.CategoryId);
    }
}
