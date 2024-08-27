using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class EventAuditConfiguration : IEntityTypeConfiguration<EventAudit>
{
    public void Configure(EntityTypeBuilder<EventAudit> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(EventAudit)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(e => new { e.EventDateOnly, e.EntityType }, $"IX_{PluralizationProvider.Pluralize(nameof(EventAudit))}");
        builder.HasIndex(e => new { e.EventDateOnly }, $"IX_{PluralizationProvider.Pluralize(nameof(EventAudit))}_DateOnly");

        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.EventId).IsRequired();
        builder.Property(d => d.EventName).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(d => d.EventDescription).HasMaxLength(3000).IsUnicode(false).IsRequired(false);
        builder.Property(d => d.Success).IsRequired();
        builder.Property(d => d.EntityType).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(d => d.EntityId).IsRequired();
        builder.Property(d => d.EventDate).HasComment("Date and Time when event occurred. Should be specified in UTC format.")
            .HasDefaultValueSql("(sysutcdatetime())");
        builder.Property(d => d.EventDateOnly).HasColumnType("date")
            .HasComment("Date only portion of EventDate. Used for creating the indexed value.")
            .HasDefaultValueSql("(CONVERT (date, SYSUTCDATETIME()))");
    }
}
