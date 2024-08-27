
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class JobTypePayRuleConfiguration : IEntityTypeConfiguration<JobTypePayRule>
{
    public void Configure(EntityTypeBuilder<JobTypePayRule> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(JobTypePayRule)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(p => p.RowId).IsClustered(true);

        builder.Property(p => p.RowId).ValueGeneratedOnAdd();
        builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");
        builder.Property(j => j.JobTypes).HasMaxLength(255);

        builder.HasOne(j => j.RateIncrease)
            .WithMany()
            .HasForeignKey(j => j.RateIncreaseId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypePayRules_RateIncrease");
        builder.HasOne(j => j.SpecialPay)
            .WithMany()
            .HasForeignKey(j => j.SpecialPayId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypePayRules_SpecialPay");
    }
}
