using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;
internal class WorkPeriodJobTypeStepConfiguration : IEntityTypeConfiguration<WorkPeriodJobTypeStep>
{
    public void Configure(EntityTypeBuilder<WorkPeriodJobTypeStep> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(WorkPeriodJobTypeStep)), CommonValues.Schema, t => t.HasTrigger("Enable_Nondeterminate_Computed_Column"));

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");
        builder.Property(u => u.ActivatedOn).IsRequired(true);
        builder.Property(u => u.ActiveSince).HasComputedColumnSql($"[{CommonValues.Schema}].[WorkPeriodJobTypeStepActiveSince]([PunchEntryId], [EquipmentsToUserId], [ActivatedOn])", false);
        builder.Property(u => u.DeactivatedOn).IsRequired(false);

        builder.HasOne(b => b.JobType).WithMany()
            .HasForeignKey(b => b.JobTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodJobTypes_JobTypes");

        builder.HasOne(b => b.JobStep).WithMany()
            .HasForeignKey(b => b.JobStepId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodJobTypes_JobSteps");

        builder.HasOne(b => b.WorkPeriod)
            .WithMany(w => w.WorkPeriodJobTypeSteps)
            .HasForeignKey(b => b.WorkPeriodId)
            .HasPrincipalKey(w => w.Id)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodJobTypes_WorkPeriods");

        builder.HasOne(b => b.PunchEntry)
            .WithMany()
            .HasForeignKey(b => b.PunchEntryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodJobTypes_PunchEntries");

        builder.HasOne(b => b.EquipmentsToUser)
            .WithMany()
            .HasForeignKey(b => b.EquipmentsToUserId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodJobTypes_EquipmentsToUsers");

        builder.HasOne(b => b.DeactivatedBy)
            .WithMany()
            .HasForeignKey(b => b.DeactivatedById)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodJobTypes_Users_DeactivatedBy");
    }
}
