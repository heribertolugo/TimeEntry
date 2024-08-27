using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;
internal class SentEmailConfiguration : IEntityTypeConfiguration<SentEmail>
{
    public void Configure(EntityTypeBuilder<SentEmail> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(SentEmail)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");

        builder.Property(s => s.SentOn).IsRequired(true);
        builder.Property(s => s.SentTo).IsRequired(true).HasMaxLength(50).IsUnicode(false);
        builder.Property(s => s.Signature).IsRequired(true).HasMaxLength(512).IsUnicode(false);
        builder.Property(s => s.Subject).IsRequired(true).HasMaxLength(60).IsUnicode(false);
        builder.Property(s => s.Message).IsRequired(true).HasMaxLength(300).IsUnicode(false);
    }
}
