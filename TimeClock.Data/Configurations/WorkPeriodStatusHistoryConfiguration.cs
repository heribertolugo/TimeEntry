using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;
internal class WorkPeriodStatusHistoryConfiguration : IEntityTypeConfiguration<WorkPeriodStatusHistory>
{
    public void Configure(EntityTypeBuilder<WorkPeriodStatusHistory> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(WorkPeriodStatusHistory)), CommonValues.Schema);

        builder.HasKey(h => h.Id).IsClustered(false);
        builder.HasAlternateKey(h => h.RowId).IsClustered(true);

        builder.Property(h => h.RowId).ValueGeneratedOnAdd();
        builder.Property(h => h.Id).HasDefaultValueSql("NEWID()");
        builder.Property(h => h.Status).HasMaxLength(20).HasConversion<string>();
        builder.Property(h => h.DateTime);

        builder.HasOne(h => h.WorkPeriod)
            .WithMany(w => w.WorkPeriodStatusHistories)
            .HasForeignKey(h => h.WorkPeriodId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodStatusHistory_WorkPeriod");
    }
}
