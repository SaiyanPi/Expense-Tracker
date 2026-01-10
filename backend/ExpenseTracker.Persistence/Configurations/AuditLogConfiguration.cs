using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;
public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        // Primary key
        builder.HasKey(x => x.Id);

        // Enum as string
        builder.Property(x => x.EntityName)
                .HasConversion<string>() // store enum as string
                .HasMaxLength(50)
                .IsRequired();

        // EntityId is required
        builder.Property(x => x.EntityId)
                .IsRequired();

        // Action enum stored as string
        builder.Property(x => x.Action)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

        // OldValues / NewValues optional
        builder.Property(x => x.OldValues)
                .HasMaxLength(4000);

        builder.Property(x => x.NewValues)
                .HasMaxLength(4000);

        // UserId optional
        builder.Property(x => x.UserId)
                .HasMaxLength(450); // matches typical Identity user id length

        // CreatedAt required
        builder.Property(x => x.CreatedAt)
                .IsRequired();

        // CorrelationId required
        builder.Property(x => x.CorrelationId)
                .HasMaxLength(100)
                .IsRequired();

        // Http metadata optional
        builder.Property(x => x.HttpMethod)
                .HasMaxLength(10);

        builder.Property(x => x.RequestPath)
                .HasMaxLength(200);

        builder.Property(x => x.ClientIp)
                .HasMaxLength(50);

        builder.Property(x => x.UserAgent)
                .HasMaxLength(500);

        // Indexes

        // Fast ordering + retention cleanup
        builder.HasIndex(x => x.CreatedAt);

        // Admin filtering by entity
        builder.HasIndex(x => new { x.EntityName, x.CreatedAt });

        // User-based audit lookup
        builder.HasIndex(x => new { x.UserId, x.CreatedAt });

        // Activity timeline per entity
        builder.HasIndex(x => new { x.EntityName, x.EntityId, x.CreatedAt });
    }
}