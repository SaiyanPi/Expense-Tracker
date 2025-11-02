using ExpenseTracker.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(u => u.Categories)
            .WithOne()  // no reverse nav in Category
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.Expenses)
            .WithOne()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        //📝 forced to configure the ApplicationUser and Category/Expense relationship here even though
        // Category and Expense are the dependent entity because we can't reference ApplicationUser
        // from Domain layer(breaks CLEAN architecture.)
    }
}
