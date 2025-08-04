using ExpenseTracker.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ECategoryConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(u => u.Categories)
            .WithOne()  // no reverse nav in Category
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(u => u.Expenses)
            .WithOne()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        //📝 forced to configure the ApplicationUser and Category/Expense relationship here even though
        // Category and Expense are the dependent entity because we can't reference ApplicationUser
        // from Domain layer(breaks CLEAN architecture.)
    }
}
