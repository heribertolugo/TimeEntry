using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class WorkPeriodConfiguration : IEntityTypeConfiguration<WorkPeriod>
{
    internal static readonly string[] _indexNames2 = ["WorkDate"];

    public void Configure(EntityTypeBuilder<WorkPeriod> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(WorkPeriod)), CommonValues.Schema, t => t.HasTrigger("Enable_Nondeterminate_Computed_Column"));

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId);

        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();

        builder.HasIndex(_indexNames2, "IC_WorkPeriods_WorkDate").IsClustered();
        builder.HasIndex(d => d.UserId).IsClustered(false);

        builder.Property(d => d.HoursWorked);
        builder.Property(d => d.Purpose).HasMaxLength(20).IsUnicode(false).HasConversion<string>();
        builder.Property(d => d.WorkDate).HasColumnType("date");
        builder.Property(d => d.IsPreviousMissingPunch).HasComputedColumnSql($"[{CommonValues.Schema}].[IsPreviousMissingPunch]([Id])", false);

        builder.HasOne(d => d.User).WithMany(u => u.WorkPeriods)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriods_Users");
    }
}
