using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;

internal class JdeCustomCodeConfiguration : IEntityTypeConfiguration<JdeCustomCode>
{
    public void Configure(EntityTypeBuilder<JdeCustomCode> builder) 
    {
        builder.HasKey(e => new { e.ProductCode, e.Codes, e.Code }).HasName("F0005_PK");

        builder.ToTable("F0005", CommonValues.JdeCtlSchema);

        builder.HasIndex(e => new { e.ProductCode, e.Codes, e.Drdl02, e.Code }, "F0005_2").IsUnique();

        builder.HasIndex(e => new { e.ProductCode, e.Codes, e.Description }, "F0005_3");

        builder.Property(e => e.ProductCode)
                .HasMaxLength(4)
                .IsFixedLength()
                .HasColumnName("DRSY");
        builder.Property(e => e.Codes)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("DRRT");
        builder.Property(e => e.Code)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("DRKY");
        builder.Property(e => e.Description)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("DRDL01");
        builder.Property(e => e.Drdl02)
                .HasMaxLength(30)
                .IsFixedLength()
                .HasColumnName("DRDL02");
        builder.Property(e => e.Drhrdc)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("DRHRDC");
        builder.Property(e => e.Drjobn)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("DRJOBN");
        builder.Property(e => e.Drpid)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("DRPID");
        builder.Property(e => e.Drsphd)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("DRSPHD");
        builder.Property(e => e.Drudco)
                .HasMaxLength(1)
                .IsFixedLength()
                .HasColumnName("DRUDCO");
        builder.Property(e => e.Drupmj)
                .HasPrecision(6)
                .HasColumnName("DRUPMJ");
        builder.Property(e => e.Drupmt)
                .HasColumnType("NUMBER")
                .HasColumnName("DRUPMT");
        builder.Property(e => e.Druser)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("DRUSER");
    }
}