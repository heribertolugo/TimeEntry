using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class EquipmentTypeConfiguration : IEntityTypeConfiguration<EquipmentType>
{
    public void Configure(EntityTypeBuilder<EquipmentType> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(EquipmentType)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(e => e.Name, "IX_EquipmentTypes").IsUnique();

        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Name).HasMaxLength(30).IsUnicode(false);
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.JdeId).IsRequired(false);
    }
}
