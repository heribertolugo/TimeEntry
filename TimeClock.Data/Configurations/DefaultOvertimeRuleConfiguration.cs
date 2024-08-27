using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

public class DefaultOvertimeRuleConfiguration : IEntityTypeConfiguration<DefaultOvertimeRule>
{
    public void Configure(EntityTypeBuilder<DefaultOvertimeRule> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(DefaultOvertimeRule)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.AfterHours).HasPrecision(4, 2);
        builder.Property(d => d.OvertimeThreshold);

        builder.HasOne(d => d.SpecialPay)
            .WithMany()
            .HasForeignKey(d => d.SpecialPayId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_DefaultOvertimeRule_SpecialPay");
    }
}
