
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class SpecialPayConfiguration : IEntityTypeConfiguration<SpecialPay>
{
    public void Configure(EntityTypeBuilder<SpecialPay> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(SpecialPay)), CommonValues.Schema, 
            t => t.HasCheckConstraint("CK_HolidaySaturdaySunday_NotAllNull"
            , "HolidayId IS NOT NULL OR SaturdayId IS NOT NULL OR SundayId IS NOT NULL"));

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(p => p.RowId).IsClustered(true);

        builder.Property(p => p.RowId).ValueGeneratedOnAdd();
        builder.Property(p => p.Id).HasDefaultValueSql("NEWID()");

        builder.HasOne(s => s.Holiday)
            .WithMany()
            .HasForeignKey(s => s.HolidayId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SpecialPay_Holiday_RateIncrease");
        builder.HasOne(s => s.Saturday)
            .WithMany()
            .HasForeignKey(s => s.SaturdayId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SpecialPay_Saturday_RateIncrease");
        builder.HasOne(s => s.Sunday)
            .WithMany()
            .HasForeignKey(s => s.SundayId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_SpecialPay_Sunday_RateIncrease");
    }
}