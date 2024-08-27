using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class UserClaimsConfiguration : IEntityTypeConfiguration<UserClaim>
{
    public void Configure(EntityTypeBuilder<UserClaim> builder)
    {
        builder.ToTable(PluralizationProvider.Pluralize(nameof(UserClaim)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.HasIndex(c => c.UserId).IsUnique(false);
        builder.HasIndex(c => new { c.UserId, c.AuthorizationClaimId }).IsUnique();

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");

        builder.HasOne(b => b.User).WithMany(u => u.UserClaims)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_UserClaims_Users");
        builder.HasOne(b => b.AuthorizationClaim).WithMany(a => a.UserClaims)
            .HasForeignKey(b => b.AuthorizationClaimId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_AuthorizationClaims_Users");
    }
}
