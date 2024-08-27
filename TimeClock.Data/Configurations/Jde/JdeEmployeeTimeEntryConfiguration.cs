using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;
internal class JdeEmployeeTimeEntryConfiguration : IEntityTypeConfiguration<JdeEmployeeTimeEntry>
{
    public void Configure(EntityTypeBuilder<JdeEmployeeTimeEntry> builder)
    {
        builder.HasKey(e => new { e.EmployeeId, e.Ytdwk, e.Ytprtr }).HasName("F06116_PK");

        builder.ToTable("F06116", CommonValues.JdeSchema);

        builder.HasIndex(e => new { e.EmployeeId, e.Ytckcn, e.Ytdwk }, "F06116_10");

        builder.HasIndex(e => e.Ytusr, "F06116_11");

        builder.HasIndex(e => new { e.Ythmcu, e.Ytmail }, "F06116_12");

        builder.HasIndex(e => new { e.Ytdwk, e.Yticu, e.Ytpdba, e.EmployeeId }, "F06116_13");

        builder.HasIndex(e => new { e.Yticu, e.Ytdwk, e.EmployeeId }, "F06116_14");

        builder.HasIndex(e => e.Ytprtr, "F06116_15");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytpdba }, "F06116_16");

        builder.HasIndex(e => new { e.Ytmcu, e.Ytobj, e.Ytsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu, e.Ytpdba, e.Ytjbcd, e.Ytjbst }, "F06116_17");

        builder.HasIndex(e => new { e.Ytgmcu, e.Ytgobj, e.Ytgsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu, e.Ytpdba, e.Ytjbcd, e.Ytjbst }, "F06116_18");

        builder.HasIndex(e => new { e.Ytmcu, e.Ytqobj, e.Ytsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu }, "F06116_19");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytdwk, e.Ytphrt }, "F06116_2");

        builder.HasIndex(e => new { e.Ytsbl, e.Ytsblt, e.Ytopsq }, "F06116_20");

        builder.HasIndex(e => new { e.Ytgmcu, e.Ytqobj, e.Ytgsub, e.Ytsbl, e.Ytsblt, e.Ytdgl, e.Ytgicu }, "F06116_21");

        builder.HasIndex(e => new { e.Yticu, e.EmployeeId, e.Ytprtr }, "F06116_22");

        builder.HasIndex(e => new { e.EmployeeId, e.Yttskid }, "F06116_23");

        builder.HasIndex(e => new { e.Yttskid, e.EmployeeId, e.Ytdwk, e.Ytpdba }, "F06116_24");

        builder.HasIndex(e => new { e.Ytotrulecd, e.EmployeeId }, "F06116_25");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytpdba, e.Yttaxx }, "F06116_26");

        builder.HasIndex(e => new { e.Ytckcn, e.Ytusr, e.EmployeeId, e.Ytpdba, e.Ytdwk, e.Ytprtr }, "F06116_27");

        builder.HasIndex(e => new { e.EmployeeId, e.Yttcde }, "F06116_28");

        builder.HasIndex(e => new { e.EmployeeId, e.Yttelkpp }, "F06116_29");

        builder.HasIndex(e => e.Ytgicu, "F06116_3");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytpaylia }, "F06116_30");

        builder.HasIndex(e => e.Ytlttp, "F06116_31");

        builder.HasIndex(e => e.Ytinstid, "F06116_32");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytbptx }, "F06116_33");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytttap, e.Ytlded, e.Ytpdba }, "F06116_34");

        builder.HasIndex(e => new { e.Yticu, e.Ytuser, e.EmployeeId, e.Ytdwk, e.Ytprtr }, "F06116_35");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytpdba, e.Ytdw }, "F06116_36");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytdwk, e.Yteqcg }, "F06116_37");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytckcn, e.Ytwst, e.Ytwcnt, e.Ytwcty }, "F06116_4");

        builder.HasIndex(e => new { e.EmployeeId, e.Ytckcn, e.Ytpdba, e.Ytshrt }, "F06116_5");

        builder.HasIndex(e => new { e.Ytmcuo, e.Ytdwk, e.Yticu, e.Ytpdba, e.EmployeeId, e.Ytsub }, "F06116_6");

        builder.HasIndex(e => new { e.Yticu, e.Ytprtr }, "F06116_7");

        builder.HasIndex(e => new { e.Ythmco, e.EmployeeId, e.Ytwst, e.Ytwcmp, e.Ytwet, e.Ytrtwc, e.Ytdedm }, "F06116_8");

        builder.HasIndex(e => new { e.Ytmcuo, e.EmployeeId, e.Ytmail, e.Ytjbcd }, "F06116_9");

        builder.Property(e => e.EmployeeId)
            .HasColumnType("NUMBER")
            .HasColumnName("YTAN8");
        builder.Property(e => e.Ytdwk)
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
        builder.Property(e => e.Ytaco)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTACO");
        builder.Property(e => e.Ytactb)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("YTACTB");
        builder.Property(e => e.Ytadv)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTADV");
        builder.Property(e => e.Ytai)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTAI");
        builder.Property(e => e.Ytalph)
            .HasMaxLength(40)
            .IsFixedLength()
            .HasColumnName("YTALPH");
        builder.Property(e => e.Ytalt0)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTALT0");
        builder.Property(e => e.Ytam)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTAM");
        builder.Property(e => e.Ytani)
            .HasMaxLength(29)
            .IsFixedLength()
            .HasColumnName("YTANI");
        builder.Property(e => e.Ytann8)
            .HasColumnType("NUMBER")
            .HasColumnName("YTANN8");
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
        builder.Property(e => e.Ytbfa)
            .HasColumnType("NUMBER")
            .HasColumnName("YTBFA");
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
        builder.Property(e => e.Ytco)
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
        builder.Property(e => e.Ytctry)
            .HasColumnType("NUMBER")
            .HasColumnName("YTCTRY");
        builder.Property(e => e.Ytdedm)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEDM");
        builder.Property(e => e.Ytdep1)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEP1");
        builder.Property(e => e.Ytdep2)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEP2");
        builder.Property(e => e.Ytdep3)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEP3");
        builder.Property(e => e.Ytdep4)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEP4");
        builder.Property(e => e.Ytdep5)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEP5");
        builder.Property(e => e.Ytdep6)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDEP6");
        builder.Property(e => e.Ytdgl)
            .HasPrecision(6)
            .HasColumnName("YTDGL");
        builder.Property(e => e.Ytdicj)
            .HasPrecision(6)
            .HasColumnName("YTDICJ");
        builder.Property(e => e.Ytdpa)
            .HasColumnType("NUMBER")
            .HasColumnName("YTDPA");
        builder.Property(e => e.Ytdtab)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("YTDTAB");
        builder.Property(e => e.Ytdtbt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTDTBT");
        builder.Property(e => e.Ytdtsp)
            .HasColumnType("NUMBER")
            .HasColumnName("YTDTSP");
        builder.Property(e => e.Ytdw)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTDW");
        builder.Property(e => e.Ytepa)
            .HasColumnType("NUMBER")
            .HasColumnName("YTEPA");
        builder.Property(e => e.Yteqcg)
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
        builder.Property(e => e.Ytficm)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTFICM");
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
        builder.Property(e => e.Ythmco)
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
        builder.Property(e => e.Yticc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTICC");
        builder.Property(e => e.Ytics)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTICS");
        builder.Property(e => e.Yticu)
            .HasColumnType("NUMBER")
            .HasColumnName("YTICU");
        builder.Property(e => e.Ytiiap)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTIIAP");
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
        builder.Property(e => e.Ytjbcd)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTJBCD");
        builder.Property(e => e.Ytjblc)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTJBLC");
        builder.Property(e => e.Ytjbst)
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
        builder.Property(e => e.Ytohf)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTOHF");
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
        builder.Property(e => e.Ytpalf)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("YTPALF");
        builder.Property(e => e.Ytpayg)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPAYG");
        builder.Property(e => e.Ytpaylia)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPAYLIA");
        builder.Property(e => e.Ytpaym)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPAYM");
        builder.Property(e => e.Ytpayn)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPAYN");
        builder.Property(e => e.Ytpb)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPB");
        builder.Property(e => e.Ytpbrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPBRT");
        builder.Property(e => e.Ytpck)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTPCK");
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
        builder.Property(e => e.Ytpgrp)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTPGRP");
        builder.Property(e => e.Ytphrt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPHRT");
        builder.Property(e => e.Ytphrw)
            .HasColumnType("NUMBER")
            .HasColumnName("YTPHRW");
        builder.Property(e => e.Ytpid)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("YTPID");
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
        builder.Property(e => e.Ytsec)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSEC$");
        builder.Property(e => e.Ytsflg)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSFLG");
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
        builder.Property(e => e.Ytstip)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTSTIP");
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
        builder.Property(e => e.Yttcani)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCANI");
        builder.Property(e => e.Yttcbr)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCBR");
        builder.Property(e => e.Yttcde)
            .HasColumnType("NUMBER")
            .HasColumnName("YTTCDE");
        builder.Property(e => e.Yttcfd)
            .HasPrecision(6)
            .HasColumnName("YTTCFD");
        builder.Property(e => e.Yttchb)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCHB");
        builder.Property(e => e.Yttchc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCHC");
        builder.Property(e => e.Yttcjl)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCJL");
        builder.Property(e => e.Yttcjs)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCJS");
        builder.Property(e => e.Yttcjt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCJT");
        builder.Property(e => e.Yttcpc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCPC");
        builder.Property(e => e.Yttcrflg)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCRFLG");
        builder.Property(e => e.Yttcrt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCRT");
        builder.Property(e => e.Yttcsc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCSC");
        builder.Property(e => e.Yttcsd)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCSD");
        builder.Property(e => e.Yttctd)
            .HasPrecision(6)
            .HasColumnName("YTTCTD");
        builder.Property(e => e.Yttcun)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCUN");
        builder.Property(e => e.Yttcwc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCWC");
        builder.Property(e => e.Yttcws)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTCWS");
        builder.Property(e => e.Yttelkflg)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTELKFLG");
        builder.Property(e => e.Yttelkpp)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTTELKPP");
        builder.Property(e => e.Yttskid)
            .HasColumnType("NUMBER")
            .HasColumnName("YTTSKID");
        builder.Property(e => e.Yttt01)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT01");
        builder.Property(e => e.Yttt02)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT02");
        builder.Property(e => e.Yttt03)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT03");
        builder.Property(e => e.Yttt04)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT04");
        builder.Property(e => e.Yttt05)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT05");
        builder.Property(e => e.Yttt06)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT06");
        builder.Property(e => e.Yttt07)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT07");
        builder.Property(e => e.Yttt08)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT08");
        builder.Property(e => e.Yttt09)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT09");
        builder.Property(e => e.Yttt10)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT10");
        builder.Property(e => e.Yttt11)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT11");
        builder.Property(e => e.Yttt12)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT12");
        builder.Property(e => e.Yttt13)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT13");
        builder.Property(e => e.Yttt14)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT14");
        builder.Property(e => e.Yttt15)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("YTTT15");
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
        builder.Property(e => e.Ytun)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("YTUN");
        builder.Property(e => e.Ytupmj)
            .HasPrecision(6)
            .HasColumnName("YTUPMJ");
        builder.Property(e => e.Ytupmt)
            .HasColumnType("NUMBER")
            .HasColumnName("YTUPMT");
        builder.Property(e => e.Ytuser)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("YTUSER");
        builder.Property(e => e.Ytusr)
            .HasMaxLength(18)
            .IsFixedLength()
            .HasColumnName("YTUSR");
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
        builder.Property(e => e.Ytws)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTWS");
        builder.Property(e => e.Ytwst)
            .HasColumnType("NUMBER")
            .HasColumnName("YTWST");
        builder.Property(e => e.Ytyst)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTYST");
        builder.Property(e => e.Ytyst1)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("YTYST1");
    }
}
