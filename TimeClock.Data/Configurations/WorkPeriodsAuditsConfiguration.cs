using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class WorkPeriodsAuditsConfiguration : IEntityTypeConfiguration<WorkPeriodsAudit>
{
    public void Configure(EntityTypeBuilder<WorkPeriodsAudit> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(WorkPeriodsAudit)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId);

        builder.Property(d => d.Id).HasDefaultValueSql("NEWID()");
        builder.Property(d => d.RowId).ValueGeneratedOnAdd();
        builder.Property(d => d.Field).HasColumnType("varchar(25)");
        builder.Property(d => d.OldValue).HasColumnType("varchar(25)");
        builder.Property(d => d.NewValue).HasColumnType("varchar(25)");

        builder.HasOne(a => a.User)
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_WorkPeriodsAudits_Users");
    }
}
