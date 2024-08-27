using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using System.Collections;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class JobTypeStepToUserConfiguration : IEntityTypeConfiguration<JobTypeStepToUser>
{
    public void Configure(EntityTypeBuilder<JobTypeStepToUser> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(JobTypeStepToUser)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");

        builder.Property(j => j.IsActive).HasDefaultValue(true);

        builder.HasIndex(i => new { i.UserId, i.JobTypeId, i.JobStepId }).IsUnique(true);

        builder.HasOne(b => b.JobType).WithMany()
            .HasForeignKey(b => b.JobTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypeStepToUsers_JobTypes");

        builder.HasOne(b => b.JobStep).WithMany()
            .HasForeignKey(b => b.JobStepId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypeStepToUsers_JobSteps");

        builder.HasOne(b => b.User)
            .WithMany(u => u.JobTypeSteps)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypeStepToUsers_Users");
    }
}
