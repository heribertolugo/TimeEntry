using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;
internal class JdeNextNumbersConfiguration : IEntityTypeConfiguration<JdeNextNumber>
{
    public void Configure(EntityTypeBuilder<JdeNextNumber> builder)
    {
        builder.HasKey(e => e.ProductCode).HasName("F0002_PK");

        builder.ToTable("F0002", CommonValues.JdeCtlSchema);

        builder.Property(e => e.ProductCode)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("NNSY");
        builder.Property(e => e.Nnck01)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK01");
        builder.Property(e => e.Nnck02)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK02");
        builder.Property(e => e.Nnck03)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK03");
        builder.Property(e => e.Nnck04)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK04");
        builder.Property(e => e.Nnck05)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK05");
        builder.Property(e => e.Nnck06)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK06");
        builder.Property(e => e.Nnck07)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK07");
        builder.Property(e => e.Nnck08)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK08");
        builder.Property(e => e.Nnck09)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK09");
        builder.Property(e => e.Nnck10)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("NNCK10");
        builder.Property(e => e.NextNumberRange1)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN001")
            .IsConcurrencyToken();
        builder.Property(e => e.NextNumberRange2)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN002")
            .IsConcurrencyToken();
        builder.Property(e => e.Nnn003)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN003");
        builder.Property(e => e.Nnn004)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN004");
        builder.Property(e => e.Nnn005)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN005");
        builder.Property(e => e.Nnn006)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN006");
        builder.Property(e => e.Nnn007)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN007");
        builder.Property(e => e.Nnn008)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN008");
        builder.Property(e => e.Nnn009)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN009");
        builder.Property(e => e.Nnn010)
            .HasColumnType("NUMBER")
            .HasColumnName("NNN010");
        builder.Property(e => e.Nnud01)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD01");
        builder.Property(e => e.Nnud02)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD02");
        builder.Property(e => e.Nnud03)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD03");
        builder.Property(e => e.Nnud04)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD04");
        builder.Property(e => e.Nnud05)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD05");
        builder.Property(e => e.Nnud06)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD06");
        builder.Property(e => e.Nnud07)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD07");
        builder.Property(e => e.Nnud08)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD08");
        builder.Property(e => e.Nnud09)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD09");
        builder.Property(e => e.Nnud10)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("NNUD10");
    }
}
