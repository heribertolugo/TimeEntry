using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class JobTypeStepToEquipmentConfiguration : IEntityTypeConfiguration<JobTypeStepToEquipment>
{
    public void Configure(EntityTypeBuilder<JobTypeStepToEquipment> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(JobTypeStepToEquipment)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");
        builder.Property(u => u.UnionCode).HasMaxLength(6).IsUnicode(false).IsRequired(false);

        builder.HasOne(b => b.JobType).WithMany()
            .HasForeignKey(b => b.JobTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypeStepToEquipments_JobTypes");

        builder.HasOne(b => b.JobStep).WithMany()
            .HasForeignKey(b => b.JobStepId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypeStepToEquipments_JobSteps");

        builder.HasOne(b => b.Equipment)
            .WithMany(u => u.JobTypeStepToEquipment)
            .HasForeignKey(b => b.EquipmentId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypeStepToEquipments_Equipments");
    }
}