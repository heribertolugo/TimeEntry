using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PluralizeService.Core;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models;

namespace TimeClock.Data.Configurations;

internal class BarcodeConfiguration : IEntityTypeConfiguration<Barcode>
{
    public void Configure(EntityTypeBuilder<Barcode> builder)
    {

        builder.ToTable(PluralizationProvider.Pluralize(nameof(Barcode)), CommonValues.Schema);

        builder.HasKey(x => x.Id).IsClustered(false);
        builder.HasAlternateKey(d => d.RowId).IsClustered(true);

        builder.Property(u => u.RowId).ValueGeneratedOnAdd();
        builder.Property(u => u.Id).HasDefaultValueSql("NEWID()");
        builder.Property(e => e.Value).HasMaxLength(20).IsUnicode(false);
        builder.Property(u => u.ActivatedOn).HasDefaultValueSql("GETDATE()");
        builder.Property(u => u.DeactivatedOn).IsRequired(false);

        builder.HasOne(b => b.User).WithMany(u => u.Barcodes)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Barcodes_Users");

        builder.HasOne(b => b.DeactivatedBy).WithMany()
            .HasForeignKey(b => b.DeactivatedById)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_Barcodes_Users_DeactivatedBy");
    }
}
