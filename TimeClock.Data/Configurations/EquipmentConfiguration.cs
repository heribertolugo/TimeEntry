using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(Equipment)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(e => new { e.Sku, e.Name }, "IX_Equipments").IsUnique();

        builder.Property(e => e.Description).HasMaxLength(50).IsUnicode(false);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Name).HasMaxLength(30).IsUnicode(false);
        builder.Property(e => e.Sku).HasMaxLength(15).IsUnicode(false);
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.JdeId).IsRequired(false);

        builder.HasOne(d => d.EquipmentType).WithMany(p => p.Equipment)
            .HasForeignKey(d => d.EquipmentTypeId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Equipments_EquipmentTypes");
    }
}
