using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class PaySettingsConfiguration : IEntityTypeConfiguration<PaySettings>
{
    public void Configure(EntityTypeBuilder<PaySettings> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(PaySettings)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(p => p.RowId).IsClustered(true);

        builder.Property(p => p.RowId).ValueGeneratedOnAdd();
        builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");
        builder.Property(p => p.StraightTimeDba);
        builder.Property(p => p.OvertimeDba);
        builder.Property(p => p.StraightTimeDba);
        builder.Property(p => p.OvertimeDba);
        builder.Property(p => p.StraightTimeGlCode);
        builder.Property(p => p.OvertimeGlCode);
        builder.Property(p => p.CompanyId);

        builder.HasOne(p => p.DefaultOvertimeRule)
            .WithOne(s => s.PaySettings)
            .HasForeignKey<DefaultOvertimeRule>(p => p.PaySettingsId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_PaySettings_DefaultOvertimeRules");

        builder.HasMany(p => p.SpecialOvertimeRules)
            .WithOne(s => s.PaySettings)
            .HasForeignKey(s => s.PaySettingsId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SpecialOvertimeRules_PaySettings");

        builder.HasMany(p => p.JobTypePayRules)
            .WithOne(j => j.PaySettings)
            .HasForeignKey(j => j.PaySettingsId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_JobTypePayRules_PaySettings");
    }
}
