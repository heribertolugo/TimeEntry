using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class AuthorizationClaimConfiguration : IEntityTypeConfiguration<AuthorizationClaim>
{
    public void Configure(EntityTypeBuilder<AuthorizationClaim> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(AuthorizationClaim)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Type).HasMaxLength(30).IsUnicode(false);
        builder.Property(e => e.Value).HasMaxLength(35).IsUnicode(false);
    }
}
