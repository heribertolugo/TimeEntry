using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class UnregisteredDevicesConfiguration : IEntityTypeConfiguration<UnregisteredDevice>
{
    public void Configure(EntityTypeBuilder<UnregisteredDevice> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(UnregisteredDevice)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Name).HasMaxLength(30).IsUnicode(false).IsRequired(true);
        builder.Property(e => e.RefreshToken).HasMaxLength(100).IsUnicode(false).IsRequired(false);
    }
}
