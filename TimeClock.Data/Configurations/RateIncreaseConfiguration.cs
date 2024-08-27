
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class RateIncreaseConfiguration : IEntityTypeConfiguration<RateIncrease>
{
    public void Configure(EntityTypeBuilder<RateIncrease> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(RateIncrease)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(p => p.RowId).IsClustered(true);

        builder.Property(p => p.RowId).ValueGeneratedOnAdd();
        builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");
        builder.Property(p => p.Amount).HasColumnType("smallmoney");
        builder.Property(p => p.RateIncreaseType);
    }
}
