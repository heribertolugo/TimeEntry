using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class JobTypeConfiguration : IEntityTypeConfiguration<JobType>
{
    public void Configure(EntityTypeBuilder<JobType> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(JobType)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(j => j.RowId).IsClustered(true);

        builder.Property(j => j.RowId).ValueGeneratedOnAdd();
        builder.Property(j => j.Id).HasDefaultValueSql("NEWID()");
        builder.Property(j => j.Description).HasMaxLength(50);
        builder.Property(j => j.IsActive).HasDefaultValue(true);
        builder.Property(j => j.JdeId).HasDefaultValue(null);
    }
}
