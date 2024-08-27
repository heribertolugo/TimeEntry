using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class PunchEntriesHistoryConfiguration : IEntityTypeConfiguration<PunchEntriesHistory>
{
    internal static readonly string[] _indexNames = ["IndexedDate"];

    public void Configure(EntityTypeBuilder<PunchEntriesHistory> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(PunchEntriesHistory)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId);

        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(e => e.Action).HasMaxLength(20).IsUnicode(false).HasConversion<string>();
        builder.Property(e => e.DateTime).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.EffectiveDateTime).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.PunchType).HasMaxLength(10).IsUnicode(false).HasConversion<string>();
        builder.Property(e => e.UtcTimeStamp).HasDefaultValueSql("(sysutcdatetime())");
        builder.Property(e => e.Latitude).IsRequired(false);
        builder.Property(e => e.Longitude).IsRequired(false);
        builder.Property(h => h.Note).IsRequired(false).HasMaxLength(100).IsUnicode(false);

        builder.Property<DateOnly?>("IndexedDate").HasDefaultValueSql("convert(date, getdate())");
        builder.HasIndex(_indexNames, "IC_PunchEntriesHistory_IndexedDate").IsClustered();

        builder.HasOne(d => d.ActionBy).WithMany(p => p.PunchEntriesHistories)
            .HasForeignKey(d => d.ActionById)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesHistories_Users");

        builder.HasOne(d => d.Device).WithMany(p => p.PunchEntriesHistories)
            .HasForeignKey(d => d.DeviceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesHistories_Devices");

        builder.HasOne(d => d.PunchEntry).WithMany(p => p.PunchEntriesHistories)
            .HasForeignKey(d => d.PunchEntryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesHistories_PunchEntries");

        builder.HasOne(h => h.JobType).WithMany()
            .HasForeignKey(h => h.JobTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesHistories_JobTypes");

        builder.HasOne(h => h.JobStep).WithMany()
            .HasForeignKey(h => h.JobStepId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesHistories_JobSteps");
    }
}
