using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class EquipmentsToUserConfiguration : IEntityTypeConfiguration<EquipmentsToUser>
{
    public void Configure(EntityTypeBuilder<EquipmentsToUser> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(EquipmentsToUser)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(e => e.LinkedOn).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.UnLinkedOn).HasDefaultValueSql(null);
        builder.Property(e => e.LinkedOnEffective).HasDefaultValueSql("(getdate())");
        builder.Property(e => e.UnLinkedOnEffective).HasDefaultValueSql(null);
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.JdeId).IsRequired(false);

        builder.HasOne(d => d.Equipment).WithMany(p => p.EquipmentsToUsers)
            .HasForeignKey(d => d.EquipmentId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToUsers_Equipments");

        builder.HasOne(d => d.User).WithMany(p => p.EquipmentsToUsers)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToUsers_Users");

        builder.HasOne(d => d.LinkedBy).WithMany()
            .HasForeignKey(d => d.LinkedById)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToUsers_Users_LinkedBy");

        builder.HasOne(d => d.UnlinkedBy).WithMany()
            .HasForeignKey(d => d.UnlinkedById)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToUsers_Users_UnlinkedBy")
            .IsRequired(false);

        builder.HasOne(d => d.WorkPeriod).WithMany(w => w.EquipmentsToUsers)
            .HasForeignKey(d => d.WorkPeriodId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToUsers_WorkPeriods");

        builder.HasOne(h => h.JobType).WithMany()
            .HasForeignKey(h => h.JobTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToUsers_JobTypes");

        builder.HasOne(h => h.JobStep).WithMany()
            .HasForeignKey(h => h.JobStepId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToUsers_JobSteps");
    }
}
