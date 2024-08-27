using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;

internal class JdeLocationConfiguration : IEntityTypeConfiguration<JdeLocation>
{
    public void Configure(EntityTypeBuilder<JdeLocation> builder)
    {
        builder.HasKey(e => e.BusinessUnit).HasName("F0006_PK");

        builder.ToTable("F0006", CommonValues.JdeSchema);

        builder.HasIndex(e => e.Company, "F0006_2");

        builder.HasIndex(e => new { e.BusinessUnitType, e.Company }, "F0006_4");

        //builder.HasIndex(e => new { e.BusinessUnitType, e.Mcfmod }, "F0006_5");

        //builder.HasIndex(e => new { e.Mcclnu, e.Mcpctn, e.Mcdoco }, "F0006_7");

        builder.HasIndex(e => e.Description, "F0006_8");

        builder.Property(e => e.BusinessUnit)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("MCMCU");
        //builder.Property(e => e.Mcadds)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCADDS");
        //builder.Property(e => e.Mcadjent)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCADJENT");
        //builder.Property(e => e.Mcalcl)
        //    .HasMaxLength(2)
        //    .IsFixedLength()
        //    .HasColumnName("MCALCL");
        //builder.Property(e => e.Mcals)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCALS");
        builder.Property(e => e.AddressNumber)
            .HasColumnType("NUMBER")
            .HasColumnName("MCAN8");
        //builder.Property(e => e.Mcan8gca1)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCAN8GCA1");
        //builder.Property(e => e.Mcan8gca2)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCAN8GCA2");
        //builder.Property(e => e.Mcan8gca3)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCAN8GCA3");
        //builder.Property(e => e.Mcan8gca4)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCAN8GCA4");
        //builder.Property(e => e.Mcan8gca5)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCAN8GCA5");
        //builder.Property(e => e.Mcan8o)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCAN8O");
        //builder.Property(e => e.Mcanpa)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCANPA");
        //builder.Property(e => e.Mcapsb)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCAPSB");
        //builder.Property(e => e.Mcbptp)
        //    .HasMaxLength(15)
        //    .IsFixedLength()
        //    .HasColumnName("MCBPTP");
        //builder.Property(e => e.Mcbtyp)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCBTYP");
        //builder.Property(e => e.Mcbuca)
        //    .HasMaxLength(5)
        //    .IsFixedLength()
        //    .HasColumnName("MCBUCA");
        //builder.Property(e => e.Mccac)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCCAC");
        //builder.Property(e => e.Mccc01)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC01");
        //builder.Property(e => e.Mccc02)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC02");
        //builder.Property(e => e.Mccc03)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC03");
        //builder.Property(e => e.Mccc04)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC04");
        //builder.Property(e => e.Mccc05)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC05");
        //builder.Property(e => e.Mccc06)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC06");
        //builder.Property(e => e.Mccc07)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC07");
        //builder.Property(e => e.Mccc08)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC08");
        //builder.Property(e => e.Mccc09)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC09");
        //builder.Property(e => e.Mccc10)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCC10");
        //builder.Property(e => e.Mccert)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCCERT");
        //builder.Property(e => e.Mcclnu)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCCLNU");
        //builder.Property(e => e.Mccnty)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCCNTY");
        builder.Property(e => e.Company)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("MCCO");
        //builder.Property(e => e.Mcct)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCCT");
        //builder.Property(e => e.Mcd1j)
        //    .HasPrecision(6)
        //    .HasColumnName("MCD1J");
        //builder.Property(e => e.Mcd2j)
        //    .HasPrecision(6)
        //    .HasColumnName("MCD2J");
        //builder.Property(e => e.Mcd3j)
        //    .HasPrecision(6)
        //    .HasColumnName("MCD3J");
        //builder.Property(e => e.Mcd4j)
        //    .HasPrecision(6)
        //    .HasColumnName("MCD4J");
        //builder.Property(e => e.Mcd5j)
        //    .HasPrecision(6)
        //    .HasColumnName("MCD5J");
        //builder.Property(e => e.Mcd6j)
        //    .HasPrecision(6)
        //    .HasColumnName("MCD6J");
        //builder.Property(e => e.Mcdc)
        //    .HasMaxLength(40)
        //    .IsFixedLength()
        //    .HasColumnName("MCDC");
        builder.Property(e => e.Description)
            .HasMaxLength(30)
            .IsFixedLength()
            .HasColumnName("MCDL01");
        //builder.Property(e => e.Mcdl02)
        //    .HasMaxLength(30)
        //    .IsFixedLength()
        //    .HasColumnName("MCDL02");
        //builder.Property(e => e.Mcdl03)
        //    .HasMaxLength(30)
        //    .IsFixedLength()
        //    .HasColumnName("MCDL03");
        //builder.Property(e => e.Mcdl04)
        //    .HasMaxLength(30)
        //    .IsFixedLength()
        //    .HasColumnName("MCDL04");
        //builder.Property(e => e.Mcdoco)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCDOCO");
        //builder.Property(e => e.Mceeo)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCEEO");
        //builder.Property(e => e.Mcerc)
        //    .HasMaxLength(2)
        //    .IsFixedLength()
        //    .HasColumnName("MCERC");
        //builder.Property(e => e.Mcexr1)
        //    .HasMaxLength(2)
        //    .IsFixedLength()
        //    .HasColumnName("MCEXR1");
        //builder.Property(e => e.Mcfmod)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCFMOD");
        //builder.Property(e => e.Mcfpdj)
        //    .HasPrecision(6)
        //    .HasColumnName("MCFPDJ");
        //builder.Property(e => e.Mcglba)
        //    .HasMaxLength(8)
        //    .IsFixedLength()
        //    .HasColumnName("MCGLBA");
        //builder.Property(e => e.Mcinta)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCINTA");
        //builder.Property(e => e.Mcintl)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCINTL");
        //builder.Property(e => e.Mciss)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCISS");
        //builder.Property(e => e.Mcjobn)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCJOBN");
        //builder.Property(e => e.Mcldm)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCLDM");
        //builder.Property(e => e.Mclf)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCLF");
        //builder.Property(e => e.Mclmth)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCLMTH");
        //builder.Property(e => e.Mcmcus)
        //    .HasMaxLength(12)
        //    .IsFixedLength()
        //    .HasColumnName("MCMCUS");
        //builder.Property(e => e.Mcnd01)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND01");
        //builder.Property(e => e.Mcnd02)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND02");
        //builder.Property(e => e.Mcnd03)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND03");
        //builder.Property(e => e.Mcnd04)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND04");
        //builder.Property(e => e.Mcnd05)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND05");
        //builder.Property(e => e.Mcnd06)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND06");
        //builder.Property(e => e.Mcnd07)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND07");
        //builder.Property(e => e.Mcnd08)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND08");
        //builder.Property(e => e.Mcnd09)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND09");
        //builder.Property(e => e.Mcnd10)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCND10");
        //builder.Property(e => e.Mcobj1)
        //    .HasMaxLength(6)
        //    .IsFixedLength()
        //    .HasColumnName("MCOBJ1");
        //builder.Property(e => e.Mcobj2)
        //    .HasMaxLength(6)
        //    .IsFixedLength()
        //    .HasColumnName("MCOBJ2");
        //builder.Property(e => e.Mcobj3)
        //    .HasMaxLength(6)
        //    .IsFixedLength()
        //    .HasColumnName("MCOBJ3");
        //builder.Property(e => e.Mcpac)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCPAC");
        //builder.Property(e => e.Mcpc)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCPC");
        //builder.Property(e => e.Mcpca)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCPCA");
        //builder.Property(e => e.Mcpcc)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCPCC");
        //builder.Property(e => e.Mcpctn)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCPCTN");
        //builder.Property(e => e.Mcpecc)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCPECC");
        //builder.Property(e => e.Mcpid)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCPID");
        //builder.Property(e => e.Mcrmcu1)
        //    .HasMaxLength(12)
        //    .IsFixedLength()
        //    .HasColumnName("MCRMCU1");
        builder.Property(e => e.Division)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("MCRP01");
        //builder.Property(e => e.Mcrp02)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP02");
        //builder.Property(e => e.Mcrp03)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP03");
        //builder.Property(e => e.Mcrp04)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP04");
        //builder.Property(e => e.Mcrp05)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP05");
        //builder.Property(e => e.Mcrp06)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP06");
        //builder.Property(e => e.Mcrp07)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP07");
        //builder.Property(e => e.Mcrp08)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP08");
        //builder.Property(e => e.Mcrp09)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP09");
        //builder.Property(e => e.Mcrp10)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP10");
        //builder.Property(e => e.Mcrp11)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP11");
        //builder.Property(e => e.Mcrp12)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP12");
        //builder.Property(e => e.Mcrp13)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP13");
        //builder.Property(e => e.Mcrp14)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP14");
        //builder.Property(e => e.Mcrp15)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP15");
        //builder.Property(e => e.Mcrp16)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP16");
        builder.Property(e => e.CategoryCodeBusinessUnit17)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("MCRP17");
        //builder.Property(e => e.Mcrp18)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP18");
        //builder.Property(e => e.Mcrp19)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP19");
        //builder.Property(e => e.Mcrp20)
        //    .HasMaxLength(3)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP20");
        //builder.Property(e => e.Mcrp21)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP21");
        //builder.Property(e => e.Mcrp22)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP22");
        //builder.Property(e => e.Mcrp23)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP23");
        //builder.Property(e => e.Mcrp24)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP24");
        //builder.Property(e => e.Mcrp25)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP25");
        //builder.Property(e => e.Mcrp26)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP26");
        //builder.Property(e => e.Mcrp27)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP27");
        //builder.Property(e => e.Mcrp28)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP28");
        //builder.Property(e => e.Mcrp29)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP29");
        //builder.Property(e => e.Mcrp30)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP30");
        //builder.Property(e => e.Mcrp31)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP31");
        //builder.Property(e => e.Mcrp32)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP32");
        //builder.Property(e => e.Mcrp33)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP33");
        //builder.Property(e => e.Mcrp34)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP34");
        //builder.Property(e => e.Mcrp35)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP35");
        //builder.Property(e => e.Mcrp36)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP36");
        //builder.Property(e => e.Mcrp37)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP37");
        //builder.Property(e => e.Mcrp38)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP38");
        //builder.Property(e => e.Mcrp39)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP39");
        //builder.Property(e => e.Mcrp40)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP40");
        //builder.Property(e => e.Mcrp41)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP41");
        //builder.Property(e => e.Mcrp42)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP42");
        //builder.Property(e => e.Mcrp43)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP43");
        //builder.Property(e => e.Mcrp44)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP44");
        //builder.Property(e => e.Mcrp45)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP45");
        //builder.Property(e => e.Mcrp46)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP46");
        //builder.Property(e => e.Mcrp47)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP47");
        //builder.Property(e => e.Mcrp48)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP48");
        //builder.Property(e => e.Mcrp49)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP49");
        //builder.Property(e => e.Mcrp50)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCRP50");
        //builder.Property(e => e.Mcsbli)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCSBLI");
        builder.Property(e => e.BusinessUnitType)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("MCSTYL");
        //builder.Property(e => e.Mcsub1)
        //    .HasMaxLength(8)
        //    .IsFixedLength()
        //    .HasColumnName("MCSUB1");
        //builder.Property(e => e.Mcta)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCTA");
        //builder.Property(e => e.Mctc01)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC01");
        //builder.Property(e => e.Mctc02)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC02");
        //builder.Property(e => e.Mctc03)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC03");
        //builder.Property(e => e.Mctc04)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC04");
        //builder.Property(e => e.Mctc05)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC05");
        //builder.Property(e => e.Mctc06)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC06");
        //builder.Property(e => e.Mctc07)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC07");
        //builder.Property(e => e.Mctc08)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC08");
        //builder.Property(e => e.Mctc09)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC09");
        //builder.Property(e => e.Mctc10)
        //    .HasMaxLength(4)
        //    .IsFixedLength()
        //    .HasColumnName("MCTC10");
        //builder.Property(e => e.Mctou)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCTOU");
        //builder.Property(e => e.Mctsbu)
        //    .HasMaxLength(12)
        //    .IsFixedLength()
        //    .HasColumnName("MCTSBU");
        //builder.Property(e => e.Mctxa1)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCTXA1");
        //builder.Property(e => e.Mctxjs)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCTXJS");
        //builder.Property(e => e.Mcuafl)
        //    .HasMaxLength(1)
        //    .IsFixedLength()
        //    .HasColumnName("MCUAFL");
        //builder.Property(e => e.Mcupmj)
        //    .HasPrecision(6)
        //    .HasColumnName("MCUPMJ");
        //builder.Property(e => e.Mcupmt)
        //    .HasColumnType("NUMBER")
        //    .HasColumnName("MCUPMT");
        //builder.Property(e => e.Mcuser)
        //    .HasMaxLength(10)
        //    .IsFixedLength()
        //    .HasColumnName("MCUSER");
    }
}