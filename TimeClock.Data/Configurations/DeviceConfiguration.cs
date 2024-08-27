using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using Device = TimeClock.Data.Models.Device;

namespace TimeClock.Data.Configurations;

internal class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(Device)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(d => d.Name, "IX_Devices").IsUnique();

        builder.Property(d => d.IsActive).HasDefaultValue(true);
        builder.Property(d => d.Name).HasMaxLength(30).IsUnicode(false);
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.FailureCount);
        builder.Property(d => d.LastActionOn);
        builder.Property(d => d.LockedOutOn);
        builder.Property(d => d.RefreshToken).HasDefaultValue(null).IsRequired(false);
        builder.Property(d => d.RefreshTokenExpiration).HasDefaultValue(null).IsRequired(false);
        builder.Property(d => d.RefreshTokenIssuedOn).HasDefaultValue(null).IsRequired(false);
        builder.Property(d => d.IsPublic).HasDefaultValue(false).IsRequired(true);

        builder.HasOne(d => d.ConfiguredBy).WithMany()
            .HasForeignKey(d => d.ConfiguredById)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Devices_Users");
        builder.HasOne(d => d.DepartmentsToLocations).WithMany(p => p.Devices)
            .HasForeignKey(d => d.DepartmentsToLocationsId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Devices_DepartmentsToLocations");
    }
}
