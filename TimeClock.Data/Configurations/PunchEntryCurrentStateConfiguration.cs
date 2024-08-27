using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class PunchEntryCurrentStateConfiguration : IEntityTypeConfiguration<PunchEntriesCurrentState>
{
    public void Configure(EntityTypeBuilder<PunchEntriesCurrentState> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(PunchEntriesCurrentState)), CommonValues.Schema, t => t.HasTrigger("Enable_NonDeterminate_Calculated_Columns"));

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(b => b.PunchEntryId).IsUnique();

        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");

        builder.Property(d => d.Status).HasComputedColumnSql($"[{CommonValues.Schema}].[GetPunchStatus]([Id])", false).HasConversion<string>().HasMaxLength(5);
        builder.Property(d => d.StableStatus).HasComputedColumnSql($"[{CommonValues.Schema}].[GetStablePunchStatus]([Id])", false).HasConversion<string>().HasMaxLength(5).IsRequired(false);

        builder
            .HasOne(d => d.PunchEntry)
            .WithOne(d => d.CurrentState)
            .HasForeignKey<PunchEntriesCurrentState>(d => d.PunchEntryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesCurrentStates_PunchEntries");

        builder
            .HasOne(d => d.PunchEntriesHistory)
            .WithOne(h => h.CurrentState)
            .HasForeignKey<PunchEntriesCurrentState>(d => d.PunchEntriesHistoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesCurrentStates_PunchEntriesHistories");

        builder
            .HasOne(d => d.StablePunchEntriesHistory)
            .WithOne()
            .HasForeignKey<PunchEntriesCurrentState>(d => d.StablePunchEntriesHistoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PunchEntriesCurrentStates_PunchEntriesHistories_Stable");
    }
}
