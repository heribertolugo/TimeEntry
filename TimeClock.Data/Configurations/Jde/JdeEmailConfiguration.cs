using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;
internal class JdeEmailConfiguration : IEntityTypeConfiguration<JdeEmail>
{
    public void Configure(EntityTypeBuilder<JdeEmail> builder)
    {
        builder.HasKey(e => new { e.EmployeeId, e.Eaidln, e.EmployeeEmailId }).HasName("F01151_PK");

        builder.ToTable("F01151", CommonValues.JdeSchema);

        //builder.HasIndex(e => new { e.UserId, e.Eaidln, e.Eaetp }, "F01151_3");

        //builder.HasIndex(e => new { e.UserId, e.Eaidln, e.Eaeclass }, "F01151_4");

        //builder.HasIndex(e => new { e.UserId, e.Eaidln, e.Eaehier, e.Eaeclass }, "F01151_5");

        builder.Property(e => e.EmployeeId)
            .HasColumnType("NUMBER")
            .HasColumnName("EAAN8");
        builder.Property(e => e.Eaidln)
            .HasColumnType("NUMBER")
            .HasColumnName("EAIDLN");
        builder.Property(e => e.EmployeeEmailId)
            .HasColumnType("NUMBER")
            .HasColumnName("EARCK7");
        //builder.Property(e => e.Eacaad)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("EACAAD");
        //builder.Property(e => e.Eacfno1)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("EACFNO1");
        //builder.Property(e => e.Eaeclass)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("EAECLASS");
        //builder.Property(e => e.Eaefor)
        //    .HasMaxLength(15)
        //    .IsFixedLength()
        //    .HasColumnName("EAEFOR");
        //builder.Property(e => e.Eaehier)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("EAEHIER");
        builder.Property(e => e.Email)
            .HasMaxLength(256)
            .HasColumnName("EAEMAL");
        //builder.Property(e => e.Eaetp)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("EAETP");
        //builder.Property(e => e.Eafalge)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("EAFALGE");
        //builder.Property(e => e.Eagen1)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("EAGEN1");
        //builder.Property(e => e.Eajobn)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("EAJOBN");
        //builder.Property(e => e.AppId)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("EAPID");
        //builder.Property(e => e.Easyncs)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("EASYNCS");
        //builder.Property(e => e.UpdatedOn)
        //    .HasPrecision(6)
        //    .HasColumnName("EAUPMJ");
        //builder.Property(e => e.Eaupmt)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("EAUPMT");
        //builder.Property(e => e.UpdatedBy)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("EAUSER");
                
    }
}
