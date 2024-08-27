using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;

internal class JdeEquipmentLocationConfiguration : IEntityTypeConfiguration<JdeEquipmentLocation>
{
    public void Configure(EntityTypeBuilder<JdeEquipmentLocation> builder)
    {
        builder.HasKey(e => e.Id).HasName("F1204_PK");

        builder.ToTable("F1204", CommonValues.JdeSchema);

        builder.HasIndex(e => new { e.LocationId, e.EquipmentId, e.CurrentState }, "F1204_4");

        builder.Property(e => e.Id)
            .HasColumnType("NUMBER")
            .HasColumnName("FMNNBR");
        builder.Property(e => e.CurrentState)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("FMAL");
        builder.Property(e => e.LocationId)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("FMLOC");
        builder.Property(e => e.EquipmentId)
            .HasColumnType("NUMBER")
            .HasColumnName("FMNUMB");
        builder.Property(e => e.AddressNumber)
            .HasColumnType("NUMBER")
            .HasColumnName("FMAN8");
        builder.Property(e => e.BusinessUnit)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("FMMCU");

        builder.HasOne(l => l.Equipment)
            .WithMany(q => q.EquipmentLocations)
            .HasForeignKey(l => l.EquipmentId)
            .HasPrincipalKey(q => q.Id);

        builder.HasOne(q => q.Location)
            .WithMany(l => l.EquipmentLocations)
            .HasForeignKey(q => q.LocationId)
            .HasPrincipalKey(l => l.BusinessUnit);
    }
}
