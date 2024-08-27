using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class SpecialOvertimeRuleConfiguration : IEntityTypeConfiguration<SpecialOvertimeRule>
{
    public void Configure(EntityTypeBuilder<SpecialOvertimeRule> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(SpecialOvertimeRule)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(p => p.RowId).IsClustered(true);

        builder.Property(p => p.RowId).ValueGeneratedOnAdd();
        builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");
        builder.Property(p => p.AfterHours).HasColumnType("decimal").HasPrecision(6,3);
        builder.Property(p => p.OvertimeThreshold);
        builder.Property(p => p.ObjectsType);

        builder.HasOne(s => s.SpecialPay)
            .WithMany()
            .HasForeignKey(s => s.SpecialPayId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SpecialOvertimeRules_SpecialPay");
    }
}
