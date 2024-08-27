using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class PunchEntryConfiguration : IEntityTypeConfiguration<PunchEntry>
{
    public void Configure(EntityTypeBuilder<PunchEntry> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(PunchEntry)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId);

        builder.HasIndex(x => x.Id);
        builder.HasIndex(x => x.UserId).IsClustered();

        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");

        builder.HasOne(d => d.User).WithMany(p => p.PunchEntries)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntries_Users");

        builder.HasOne(d => d.WorkPeriod).WithMany(w => w.PunchEntries)
            .HasForeignKey(d => d.WorkPeriodId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntries_WorkPeriods");

        builder.HasOne(p => p.Device).WithMany()
            .HasForeignKey(p => p.DeviceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntries_Devices");
    }
}
