using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class JobStepConfiguration : IEntityTypeConfiguration<JobStep>
{
    public void Configure(EntityTypeBuilder<JobStep> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(JobStep)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(j => j.RowId).IsClustered(true);

        builder.Property(j => j.RowId).ValueGeneratedOnAdd();
        builder.Property(j => j.Id).HasDefaultValueSql("NEWID()");
        builder.Property(j => j.Description).HasMaxLength(50);
        builder.Property(j => j.IsActive).HasDefaultValue(true);
        builder.Property(j => j.JdeId).HasDefaultValue(null);
    }
}
