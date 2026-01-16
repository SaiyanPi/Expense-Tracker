using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public sealed class SecurityEventLogConfiguration
    : IEntityTypeConfiguration<SecurityEventLog>
{
    public void Configure(EntityTypeBuilder<SecurityEventLog> builder)
    {
        // Primary Key
        builder.HasKey(x => x.Id);

        // Enum → string conversion
        builder.Property(x => x.EventType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Outcome)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        // User info
        builder.Property(x => x.UserId)
            .HasMaxLength(50); // ASP.NET Identity compatible

        builder.Property(x => x.UserEmail)
            .HasMaxLength(50);

        // Metadata
        builder.Property(x => x.IpAddress)
            .HasMaxLength(50); // IPv6 safe

        builder.Property(x => x.Endpoint)
            .HasMaxLength(256);

        builder.Property(x => x.UserAgent)
            .HasMaxLength(512);

        builder.Property(x => x.CorrelationId)
            .HasMaxLength(50);

        builder.Property(x => x.Timestamp)
            .IsRequired();

        // Indexes for querying & audit use-cases
        builder.HasIndex(x => x.Timestamp);
        builder.HasIndex(x => new { x.EventType, x.Timestamp });
        builder.HasIndex(x => new { x.Outcome, x.Timestamp });
        builder.HasIndex(x => new { x.UserId, x.Timestamp });
    }
}