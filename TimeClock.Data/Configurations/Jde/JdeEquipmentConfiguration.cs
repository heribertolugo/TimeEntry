using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;

internal class JdeEquipmentConfiguration : IEntityTypeConfiguration<JdeEquipment>
{
    public void Configure(EntityTypeBuilder<JdeEquipment> builder)
    {
        builder.HasKey(e => e.Id).HasName("F1201_PK");

        builder.ToTable("F1201", CommonValues.JdeSchema);

        builder.HasIndex(e => e.Description1, "F1201_14");

        builder.HasIndex(e => e.EquipmentNumber, "F1201_2");

        builder.Property(e => e.Id)
            .HasColumnType("NUMBER")
            .HasColumnName("FANUMB");
        builder.Property(e => e.EquipmentNumber)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("FAAPID");
        builder.Property(e => e.Description1)
            .HasMaxLength(30)
            .IsFixedLength()
            .HasColumnName("FADL01");
        builder.Property(e => e.Description2)
            .HasMaxLength(30)
            .IsFixedLength()
            .HasColumnName("FADL02");
        builder.Property(e => e.Description3)
            .HasMaxLength(30)
            .IsFixedLength()
            .HasColumnName("FADL03");
        builder.Property(e => e.Status)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("FAEQST");
        builder.Property(e => e.FK)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("FAACL2");

        builder.HasOne(e => e.EquipmentType)
            .WithMany()
            .HasPrincipalKey(e=> e.Code)
            .HasForeignKey(q => q.FK)
            ;
    }
}
