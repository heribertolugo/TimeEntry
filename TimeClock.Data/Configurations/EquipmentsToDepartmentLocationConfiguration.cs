using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class EquipmentsToDepartmentLocationConfiguration : IEntityTypeConfiguration<EquipmentsToDepartmentLocation>
{
    public void Configure(EntityTypeBuilder<EquipmentsToDepartmentLocation> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(EquipmentsToDepartmentLocation)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(x => x.LinkedOn).HasDefaultValueSql("(getdate())");
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.JdeId).IsRequired(false);

        builder.HasOne(d => d.Equipment).WithMany(p => p.EquipmentsToLocations)
            .HasForeignKey(d => d.EquipmentId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired()
            .HasConstraintName("FK_EquipmentsToDepartmentLocations_Equipments");

        builder.HasOne(d => d.LinkedBy).WithMany(p => p.EquipmentsToDepartmentLocations)
            .HasForeignKey(d => d.LinkedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToDepartmentLocations_Users");

        builder.HasOne(d => d.DepartmentsToLocation).WithMany(d => d.EquipmentsToDepartmentLocations)
            .HasForeignKey(d => d.DepartmentsToLocationId)
            .HasPrincipalKey(l => l.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_EquipmentsToDepartmentLocations_DepartmentsToLocation");
    }
}
