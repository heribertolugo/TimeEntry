using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;
internal class JdeEmployeeTimeEntryHistoryConfiguration : IEntityTypeConfiguration<JdeEmployeeTimeEntryHistory>
{
    public void Configure(EntityTypeBuilder<JdeEmployeeTimeEntryHistory> builder)
    {
        builder.HasKey(e => new { e.EmployeeId, e.DateWorked, e.Ytprtr }).HasName("F0618_PK");

        builder.ToTable("F0618", CommonValues.JdeSchema);

        builder.HasIndex(e => new { e.Ytmcuo, e.Ytpped, e.EmployeeName, e.EmployeeId }, "F0618_10");

        builder.HasIndex(e => new { e.Ytmcu, e.Ytpped }, "F0618_11");

        builder.HasIndex(e => new { e.UnionCode, e.Ytpped, e.EmployeeId }, "F0618_12");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytckcn, e.DateWorked }, "F0618_13");

        builder.HasIndex(e => new { e.Ytmcu, e.Ytobj, e.Ytsub, e.Ytsbl }, "F0618_14");

        builder.HasIndex(e => e.Ytprtr, "F0618_15");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytckdt }, "F0618_16");

        builder.HasIndex(e => new { e.Ytgmcu, e.Ytgobj, e.Ytgsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu, e.Ytpdba, e.JobType, e.JobStep }, "F0618_18");

        builder.HasIndex(e => new { e.Ytmcu, e.Ytobj, e.Ytsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu, e.Ytpdba, e.JobType, e.JobStep }, "F0618_19");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytckcn, e.Ytpdba, e.Ytphrt }, "F0618_2");

        builder.HasIndex(e => new { e.Ytmcu, e.Ytqobj, e.Ytsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu }, "F0618_20");

        builder.HasIndex(e => new { e.Ytsbl, e.Ytsblt, e.Ytopsq }, "F0618_21");

        builder.HasIndex(e => new { e.Ytgmcu, e.Ytqobj, e.Ytgsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu }, "F0618_22");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytckcn, e.Ytpdba, e.Ytprtr }, "F0618_23");

        builder.HasIndex(e => new { e.EmployeeId, e.Yttskid }, "F0618_24");

        builder.HasIndex(e => e.Yttskid, "F0618_25");

        builder.HasIndex(e => new { e.Ytotrulecd, e.EmployeeId }, "F0618_26");

        builder.HasIndex(e => e.Ytlttp, "F0618_27");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytbptx }, "F0618_28");

        builder.HasIndex(e => new { e.Ytgicu, e.Ytmcu, e.Ytqobj, e.Ytsub, e.EquipmentId }, "F0618_29");

        builder.HasIndex(e => new { e.Ytmcu, e.DateWorked, e.EmployeeId, e.JobType }, "F0618_3");

        builder.HasIndex(e => new { e.Ytgmcu, e.DateWorked }, "F0618_4");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytpdba, e.Ythmo, e.Ytckdt }, "F0618_5");

        builder.HasIndex(e => new { e.Ytmcu, e.EmployeeId, e.DateWorked, e.Ytpdba }, "F0618_6");

        builder.HasIndex(e => new { e.Ytmcu, e.Ytsub, e.EmployeeId, e.DateWorked }, "F0618_7");

        builder.HasIndex(e => new { e.Ytmcuo, e.JobType, e.JobStep, e.EmployeeId }, "F0618_8");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytpped, e.UnionCode, e.Ytpdba }, "F0618_9");

        builder.Property(e => e.EmployeeId)
            .HasColumnType("NUMBER")
            .HasColumnName("YTAN8");
        builder.Property(e => e.DateWorked)
            .HasPrecision(6)
            .HasColumnName("YTDWK");
        builder.Property(e => e.Ytprtr)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPRTR");
        builder.Property(e => e.Ytabr1)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTABR1");
        builder.Property(e => e.Ytabr2)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTABR2");
        builder.Property(e => e.Ytabr3)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTABR3");
        builder.Property(e => e.Ytabr4)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTABR4");
        builder.Property(e => e.Ytabt1)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTABT1");
        builder.Property(e => e.Ytabt2)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTABT2");
        builder.Property(e => e.Ytabt3)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTABT3");
        builder.Property(e => e.Ytabt4)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTABT4");
        builder.Property(e => e.Ytactb)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("YTACTB");
        builder.Property(e => e.Ytaid)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("YTAID");
        builder.Property(e => e.Ytalt0)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTALT0");
        builder.Property(e => e.Ytam)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTAM");
        builder.Property(e => e.Ytapprcflg)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTAPPRCFLG");
        builder.Property(e => e.Ytaubp)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTAUBP");
        builder.Property(e => e.Ytausptww)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTAUSPTWW");
        builder.Property(e => e.Ytbdrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTBDRT");
        builder.Property(e => e.Ytbhrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTBHRT");
        builder.Property(e => e.Ytblgrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTBLGRT");
        builder.Property(e => e.Ytbptx)
            .HasColumnType("NUMBER")
            .HasColumnName("YTBPTX");
        builder.Property(e => e.Ytckcn)
            .HasColumnType("NUMBER")
            .HasColumnName("YTCKCN");
        builder.Property(e => e.Ytckcs)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("YTCKCS");
        builder.Property(e => e.Ytckdt)
            .HasPrecision(6)
            .HasColumnName("YTCKDT");
        builder.Property(e => e.Ytcmed)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTCMED");
        builder.Property(e => e.Ytcmmt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTCMMT");
        builder.Property(e => e.Ytcmth)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTCMTH");
        builder.Property(e => e.Company)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("YTCO");
        builder.Property(e => e.Ytcopb)
            .HasColumnType("NUMBER")
            .HasColumnName("YTCOPB");
        builder.Property(e => e.Ytcopr)
            .HasColumnType("NUMBER")
            .HasColumnName("YTCOPR");
        builder.Property(e => e.Ytcopx)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTCOPX");
        builder.Property(e => e.Ytcptr)
            .HasColumnType("NUMBER")
            .HasColumnName("YTCPTR");
        builder.Property(e => e.Ytcrcd)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTCRCD");
        builder.Property(e => e.Ytcrdc)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTCRDC");
        builder.Property(e => e.Ytcrfl)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTCRFL");
        builder.Property(e => e.Ytcrr)
            .HasColumnType("NUMBER")
            .HasColumnName("YTCRR");
        builder.Property(e => e.Ytdedm)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEDM");
        builder.Property(e => e.Ytdgl)
            .HasPrecision(6)
            .HasColumnName("YTDGL");
        builder.Property(e => e.Ytdpa)
            .HasColumnType("NUMBER")
            .HasColumnName("YTDPA");
        builder.Property(e => e.Ytdtbt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTDTBT");
        builder.Property(e => e.Ytdw)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDW");
        builder.Property(e => e.EquipmentId)
            .HasMaxLength(9)
            .IsFixedLength()
            .HasColumnName("YTEQCG");
        builder.Property(e => e.Yteqco)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("YTEQCO");
        builder.Property(e => e.Yteqgr)
            .HasColumnType("NUMBER")
            .HasColumnName("YTEQGR");
        builder.Property(e => e.Yteqhr)
            .HasColumnType("NUMBER")
            .HasColumnName("YTEQHR");
        builder.Property(e => e.Yteqrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTEQRT");
        builder.Property(e => e.Yteqwo)
            .HasMaxLength(9)
            .IsFixedLength()
            .HasColumnName("YTEQWO");
        builder.Property(e => e.Yterc)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTERC");
        builder.Property(e => e.Ytexr)
            .HasMaxLength(30)
            .IsFixedLength()
            .HasColumnName("YTEXR");
        builder.Property(e => e.Ytfblgrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTFBLGRT");
        builder.Property(e => e.Ytfrchgamt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTFRCHGAMT");
        builder.Property(e => e.Ytfy)
            .HasColumnType("NUMBER")
            .HasColumnName("YTFY");
        builder.Property(e => e.Ytgena)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGENA");
        builder.Property(e => e.Ytgenb)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGENB");
        builder.Property(e => e.Ytgeno)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGENO");
        builder.Property(e => e.Ytgenr)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGENR");
        builder.Property(e => e.Ytgenx)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGENX");
        builder.Property(e => e.Ytgicu)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGICU");
        builder.Property(e => e.Ytglbn)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGLBN");
        builder.Property(e => e.Ytglex)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGLEX");
        builder.Property(e => e.Ytgmcu)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTGMCU");
        builder.Property(e => e.Ytgobj)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTGOBJ");
        builder.Property(e => e.Ytgpa)
            .HasColumnType("NUMBER")
            .HasColumnName("YTGPA");
        builder.Property(e => e.Ytgsub)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("YTGSUB");
        builder.Property(e => e.CompanyHome)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("YTHMCO");
        builder.Property(e => e.Ythmcu)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTHMCU");
        builder.Property(e => e.Ythmo)
            .HasColumnType("NUMBER")
            .HasColumnName("YTHMO");
        builder.Property(e => e.Ythwpd)
            .HasColumnType("NUMBER")
            .HasColumnName("YTHWPD");
        builder.Property(e => e.Ytinea)
            .HasMaxLength(29)
            .IsFixedLength()
            .HasColumnName("YTINEA");
        builder.Property(e => e.Ytinra)
            .HasMaxLength(29)
            .IsFixedLength()
            .HasColumnName("YTINRA");
        builder.Property(e => e.Ytinstid)
            .HasMaxLength(36)
            .IsFixedLength()
            .HasColumnName("YTINSTID");
        builder.Property(e => e.Ytinva)
            .HasMaxLength(29)
            .IsFixedLength()
            .HasColumnName("YTINVA");
        builder.Property(e => e.Ytitm)
            .HasColumnType("NUMBER")
            .HasColumnName("YTITM");
        builder.Property(e => e.JobType)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTJBCD");
        builder.Property(e => e.Ytjblc)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTJBLC");
        builder.Property(e => e.JobStep)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("YTJBST");
        builder.Property(e => e.Ytld)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTLD");
        builder.Property(e => e.Ytlded)
            .HasPrecision(6)
            .HasColumnName("YTLDED");
        builder.Property(e => e.Ytldid)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("YTLDID");
        builder.Property(e => e.Ytlttp)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTLTTP");
        builder.Property(e => e.Ytmail)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("YTMAIL");
        builder.Property(e => e.Ytmcu)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTMCU");
        builder.Property(e => e.Ytmcuo)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTMCUO");
        builder.Property(e => e.Ytnmth)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTNMTH");
        builder.Property(e => e.Ytobj)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTOBJ");
        builder.Property(e => e.Ytopsq)
            .HasColumnType("NUMBER")
            .HasColumnName("YTOPSQ");
        builder.Property(e => e.Ytotrulecd)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTOTRULECD");
        builder.Property(e => e.Ytp001)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTP001");
        builder.Property(e => e.Ytp002)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTP002");
        builder.Property(e => e.Ytp003)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTP003");
        builder.Property(e => e.Ytp004)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("YTP004");
        builder.Property(e => e.EmployeeName)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTPALF");
        builder.Property(e => e.Ytpaym)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPAYM");
        builder.Property(e => e.Ytpb)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPB");
        builder.Property(e => e.Ytpbrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPBRT");
        builder.Property(e => e.Ytpcor)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPCOR");
        builder.Property(e => e.Ytpcun)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPCUN");
        builder.Property(e => e.Ytpdba)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPDBA");
        builder.Property(e => e.Ytpfrq)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPFRQ");
        builder.Property(e => e.Ytphrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPHRT");
        builder.Property(e => e.Ytphrw)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPHRW");
        builder.Property(e => e.Ytpn)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPN");
        builder.Property(e => e.Ytpos)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("YTPOS");
        builder.Property(e => e.Ytpped)
            .HasPrecision(6)
            .HasColumnName("YTPPED");
        builder.Property(e => e.Ytppp)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPPP");
        builder.Property(e => e.Ytpprt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPPRT");
        builder.Property(e => e.Ytqobj)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTQOBJ");
        builder.Property(e => e.Ytrccd)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTRCCD");
        builder.Property(e => e.Ytrchgamt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTRCHGAMT");
        builder.Property(e => e.Ytrchgmode)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTRCHGMODE");
        builder.Property(e => e.Ytrco)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("YTRCO");
        builder.Property(e => e.Ytrcpy)
            .HasColumnType("NUMBER")
            .HasColumnName("YTRCPY");
        builder.Property(e => e.Ytrilt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTRILT");
        builder.Property(e => e.Ytrkid)
            .HasColumnType("NUMBER")
            .HasColumnName("YTRKID");
        builder.Property(e => e.Ytrtwc)
            .HasColumnType("NUMBER")
            .HasColumnName("YTRTWC");
        builder.Property(e => e.Ytsaly)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSALY");
        builder.Property(e => e.Ytsamt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTSAMT");
        builder.Property(e => e.Ytsbl)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("YTSBL");
        builder.Property(e => e.Ytsblt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSBLT");
        builder.Property(e => e.Ytscrx)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSCRX");
        builder.Property(e => e.Ytsctr)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSCTR");
        builder.Property(e => e.Ytshd)
            .HasColumnType("NUMBER")
            .HasColumnName("YTSHD");
        builder.Property(e => e.Ytshft)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSHFT");
        builder.Property(e => e.Ytshrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTSHRT");
        builder.Property(e => e.Ytsub)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("YTSUB");
        builder.Property(e => e.Ytsvh)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSVH");
        builder.Property(e => e.Yttaxx)
            .HasMaxLength(20)
            .IsFixedLength()
            .HasColumnName("YTTAXX");
        builder.Property(e => e.Yttcde)
            .HasColumnType("NUMBER")
            .HasColumnName("YTTCDE");
        builder.Property(e => e.Yttcfd)
            .HasPrecision(6)
            .HasColumnName("YTTCFD");
        builder.Property(e => e.Yttctd)
            .HasPrecision(6)
            .HasColumnName("YTTCTD");
        builder.Property(e => e.Yttskid)
            .HasColumnType("NUMBER")
            .HasColumnName("YTTSKID");
        builder.Property(e => e.Ytttap)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTTAP");
        builder.Property(e => e.Ytuamt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTUAMT");
        builder.Property(e => e.Ytum)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTUM");
        builder.Property(e => e.UnionCode)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTUN");
        builder.Property(e => e.Ytuser)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("YTUSER");
        builder.Property(e => e.Ytwcam)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCAM");
        builder.Property(e => e.Ytwcbn)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCBN");
        builder.Property(e => e.Ytwcex)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCEX");
        builder.Property(e => e.Ytwcmb)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCMB");
        builder.Property(e => e.Ytwcmo)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCMO");
        builder.Property(e => e.Ytwcmp)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("YTWCMP");
        builder.Property(e => e.Ytwcmx)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCMX");
        builder.Property(e => e.Ytwcnt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCNT");
        builder.Property(e => e.Ytwcty)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWCTY");
        builder.Property(e => e.Ytwet)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTWET");
        builder.Property(e => e.Ytwr01)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("YTWR01");
        builder.Property(e => e.Ytwst)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWST");
    }
}
