using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class DepartmentsToLocationConfiguration : IEntityTypeConfiguration<DepartmentsToLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentsToLocation> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(DepartmentsToLocation)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(e => new { e.DepartmentId, e.LocationId }, "IX_DepartmentsToLocation").IsUnique();

        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");

        builder.HasOne(d => d.Department).WithMany(p => p.DepartmentsToLocations)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DepartmentsToLocations_Departments");

        builder.HasOne(d => d.Location).WithMany(p => p.DepartmentsToLocations)
            .HasForeignKey(d => d.LocationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DepartmentsToLocations_Locations");
    }
}
