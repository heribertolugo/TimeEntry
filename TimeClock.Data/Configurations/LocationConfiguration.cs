using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using Location = TimeClock.Data.Models.Location;

namespace TimeClock.Data.Configurations;

internal class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(Location)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(e => e.Name, "IX_Locations").IsUnique();

        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Name).HasMaxLength(50).IsUnicode(false);
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.JdeId).IsRequired(false);
        builder.Property(d => d.DivisionCode).IsRequired(false);
    }
}
