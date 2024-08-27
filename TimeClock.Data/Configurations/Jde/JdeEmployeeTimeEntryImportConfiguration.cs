using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeClock.Data.Helpers;
using TimeClock.Data.Models.Jde;

namespace TimeClock.Data.Configurations.Jde;
internal class JdeEmployeeTimeEntryImportConfiguration : IEntityTypeConfiguration<JdeEmployeeTimeEntryImport>
{
    public void Configure(EntityTypeBuilder<JdeEmployeeTimeEntryImport> builder)
    {
        builder.HasKey(e => new { e.EdiId, e.EdiBatchNumber, e.EdiTransactionNumber, e.EdiLineNumber }).HasName("F06116Z1_PK");

        builder.ToTable("F06116Z1", CommonValues.JdeSchema);

        builder.HasIndex(e => new { e.EmployeeId, e.SelfServiceRecordId, e.DateWorked }, "F06116Z1_10");

        builder.HasIndex(e => e.Vlinstid, "F06116Z1_11");

        builder.HasIndex(e => new { e.EmployeeId, e.DbaCode }, "F06116Z1_12");

        builder.HasIndex(e => new { e.EdiId, e.EdiBatchNumber, e.EmployeeId, e.DateWorked, e.BatchNumber, e.EdiTransactionNumber }, "F06116Z1_2");

        builder.HasIndex(e => new { e.EmployeeId, e.DateWorked, e.PayrollTransactionNumber }, "F06116Z1_4");

        builder.HasIndex(e => new { e.Vltskid, e.EmployeeId, e.DateWorked, e.DbaCode }, "F06116Z1_5");

        builder.HasIndex(e => e.EdiBatchNumber, "F06116Z1_6");

        builder.HasIndex(e => new { e.EmployeeId, e.Vledgl }, "F06116Z1_7");

        builder.HasIndex(e => new { e.Vledgl, e.EdiId, e.EdiBatchNumber, e.EdiTransactionNumber, e.EdiLineNumber }, "F06116Z1_8");

        builder.HasIndex(e => new { e.EmployeeId, e.DbaCode, e.Vlpanp }, "F06116Z1_9");

        builder.Property(e => e.EdiId)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("VLEDUS");
        builder.Property(e => e.EdiBatchNumber)
            .HasMaxLength(15)
            .IsFixedLength()
            .HasColumnName("VLEDBT");
        builder.Property(e => e.EdiTransactionNumber)
            .HasMaxLength(22)
            .IsFixedLength()
            .HasColumnName("VLEDTN");
        builder.Property(e => e.EdiLineNumber)
            .HasColumnType("NUMBER")
            .HasColumnName("VLEDLN");
        builder.Property(e => e.Vlabr1)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLABR1");
        builder.Property(e => e.Vlabr2)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLABR2");
        builder.Property(e => e.Vlabr3)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLABR3");
        builder.Property(e => e.Vlabr4)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLABR4");
        builder.Property(e => e.Vlabt1)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLABT1");
        builder.Property(e => e.Vlabt2)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLABT2");
        builder.Property(e => e.Vlabt3)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLABT3");
        builder.Property(e => e.Vlabt4)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLABT4");
        builder.Property(e => e.DbaAvailable)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLACO");
        builder.Property(e => e.Vlactb)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("VLACTB");
        builder.Property(e => e.Vladv)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLADV");
        builder.Property(e => e.Vlai)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLAI");
        builder.Property(e => e.EmployeeName)
            .HasMaxLength(40)
            .IsFixedLength()
            .HasColumnName("VLALPH");
        builder.Property(e => e.GlAccountMode)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLAM");
        builder.Property(e => e.EmployeeId)
            .HasColumnType("NUMBER")
            .HasColumnName("VLAN8");
        builder.Property(e => e.GlAccount)
            .HasMaxLength(29)
            .IsFixedLength()
            .HasColumnName("VLANI");
        builder.Property(e => e.Vlann8)
            .HasColumnType("NUMBER")
            .HasColumnName("VLANN8");
        builder.Property(e => e.SupervisorId)
            .HasColumnType("NUMBER")
            .HasColumnName("VLANPA");
        builder.Property(e => e.Vlausptww)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLAUSPTWW");
        builder.Property(e => e.Vlbdrt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLBDRT");
        builder.Property(e => e.Vlbdsn)
            .HasMaxLength(22)
            .IsFixedLength()
            .HasColumnName("VLBDSN");
        builder.Property(e => e.Vlbfa)
            .HasColumnType("NUMBER")
            .HasColumnName("VLBFA");
        builder.Property(e => e.Vlbhrt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLBHRT");
        builder.Property(e => e.Vlblgrt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLBLGRT");
        builder.Property(e => e.Vlckcn)
            .HasColumnType("NUMBER")
            .HasColumnName("VLCKCN");
        builder.Property(e => e.Vlckdt)
            .HasPrecision(6)
            .HasColumnName("VLCKDT");
        builder.Property(e => e.Vlcmmt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLCMMT");
        builder.Property(e => e.ShiftDiffCalcSequence)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLCMTH");
        builder.Property(e => e.Company)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("VLCO");
        builder.Property(e => e.Vlcptr)
            .HasColumnType("NUMBER")
            .HasColumnName("VLCPTR");
        builder.Property(e => e.Vlcrcd)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLCRCD");
        builder.Property(e => e.Vlcrdc)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLCRDC");
        builder.Property(e => e.Vlcrfl)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLCRFL");
        builder.Property(e => e.Vlcrr)
            .HasColumnType("NUMBER")
            .HasColumnName("VLCRR");
        builder.Property(e => e.Century)
            .HasColumnType("NUMBER")
            .HasColumnName("VLCTRY");
        builder.Property(e => e.Vldedm)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLDEDM");
        builder.Property(e => e.Vldep1)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLDEP1");
        builder.Property(e => e.Vldep2)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLDEP2");
        builder.Property(e => e.Vldep3)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLDEP3");
        builder.Property(e => e.Vldep4)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLDEP4");
        builder.Property(e => e.Vldep5)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLDEP5");
        builder.Property(e => e.Vldgl)
            .HasPrecision(6)
            .HasColumnName("VLDGL");
        builder.Property(e => e.BatchDate)
            .HasPrecision(6)
            .HasColumnName("VLDICJ");
        builder.Property(e => e.Vldocm)
            .HasColumnType("NUMBER")
            .HasColumnName("VLDOCM");
        builder.Property(e => e.DistributedGrossPay)
            .HasColumnType("NUMBER")
            .HasColumnName("VLDPA");
        builder.Property(e => e.Vldtab)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("VLDTAB");
        builder.Property(e => e.Vldtbt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLDTBT");
        builder.Property(e => e.Vldtsp)
            .HasColumnType("NUMBER")
            .HasColumnName("VLDTSP");
        builder.Property(e => e.DayOfTheWeek)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLDW");
        builder.Property(e => e.DateWorked)
            .HasPrecision(6)
            .HasColumnName("VLDWK");
        builder.Property(e => e.Vledct)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLEDCT");
        builder.Property(e => e.Vleddl)
            .HasColumnType("NUMBER")
            .HasColumnName("VLEDDL");
        builder.Property(e => e.EdiSubmitDate)
            .HasPrecision(6)
            .HasColumnName("VLEDDT");
        builder.Property(e => e.EdiSendReceive)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLEDER");
        builder.Property(e => e.Vledft)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("VLEDFT");
        builder.Property(e => e.Vledgl)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLEDGL");
        builder.Property(e => e.IsEdiSuccessfullyProcessed)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLEDSP");
        builder.Property(e => e.Vledsq)
            .HasColumnType("NUMBER")
            .HasColumnName("VLEDSQ");
        builder.Property(e => e.Vledtc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLEDTC");
        builder.Property(e => e.Vledtr)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLEDTR");
        builder.Property(e => e.Vledts)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("VLEDTS");
        builder.Property(e => e.Vledty)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLEDTY");
        builder.Property(e => e.Vlepa)
            .HasColumnType("NUMBER")
            .HasColumnName("VLEPA");
        builder.Property(e => e.EquipmentUsedId)
            .HasMaxLength(9)
            .IsFixedLength()
            .HasColumnName("VLEQCG");
        builder.Property(e => e.Vleqco)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("VLEQCO");
        builder.Property(e => e.Vleqgr)
            .HasColumnType("NUMBER")
            .HasColumnName("VLEQGR");
        builder.Property(e => e.EquipmentHours)
            .HasColumnType("NUMBER")
            .HasColumnName("VLEQHR");
        builder.Property(e => e.Vleqrt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLEQRT");
        builder.Property(e => e.EquipmentWorkedOnId)
            .HasMaxLength(9)
            .IsFixedLength()
            .HasColumnName("VLEQWO");
        builder.Property(e => e.Vlerc)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLERC");
        builder.Property(e => e.TimeEntryTypeDescription)
            .HasMaxLength(30)
            .IsFixedLength()
            .HasColumnName("VLEXR");
        builder.Property(e => e.Vlfblgrt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLFBLGRT");
        builder.Property(e => e.Vlficm)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLFICM");
        builder.Property(e => e.Vlfrchgamt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLFRCHGAMT");
        builder.Property(e => e.Vlfy)
            .HasColumnType("NUMBER")
            .HasColumnName("VLFY");
        builder.Property(e => e.Vlgena)
            .HasColumnType("NUMBER")
            .HasColumnName("VLGENA");
        builder.Property(e => e.Vlgenb)
            .HasColumnType("NUMBER")
            .HasColumnName("VLGENB");
        builder.Property(e => e.Vlgeno)
            .HasColumnType("NUMBER")
            .HasColumnName("VLGENO");
        builder.Property(e => e.Vlgenr)
            .HasColumnType("NUMBER")
            .HasColumnName("VLGENR");
        builder.Property(e => e.Vlgenx)
            .HasColumnType("NUMBER")
            .HasColumnName("VLGENX");
        builder.Property(e => e.Vlgicu)
            .HasColumnType("NUMBER")
            .HasColumnName("VLGICU");
        builder.Property(e => e.Vlgmcu)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLGMCU");
        builder.Property(e => e.Vlgobj)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("VLGOBJ");
        builder.Property(e => e.GrossPay)
            .HasColumnType("NUMBER")
            .HasColumnName("VLGPA");
        builder.Property(e => e.Vlgsub)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("VLGSUB");
        builder.Property(e => e.CompanyHome)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("VLHMCO");
        builder.Property(e => e.UserBusinessUnit)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLHMCU");
        builder.Property(e => e.Vlhmo)
            .HasColumnType("NUMBER")
            .HasColumnName("VLHMO");
        builder.Property(e => e.Vlhwpd)
            .HasColumnType("NUMBER")
            .HasColumnName("VLHWPD");
        builder.Property(e => e.Vlicc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLICC");
        builder.Property(e => e.Vlics)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLICS");
        builder.Property(e => e.BatchNumber)
            .HasColumnType("NUMBER")
            .HasColumnName("VLICU");
        builder.Property(e => e.AutoPayMethods)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLIIAP");
        builder.Property(e => e.Vlinstid)
            .HasMaxLength(36)
            .IsFixedLength()
            .HasColumnName("VLINSTID");
        builder.Property(e => e.Vlitm)
            .HasColumnType("NUMBER")
            .HasColumnName("VLITM");
        builder.Property(e => e.JobType)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("VLJBCD");
        builder.Property(e => e.Vljblc)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("VLJBLC");
        builder.Property(e => e.JobStep)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("VLJBST");
        builder.Property(e => e.Vlld)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLLD");
        builder.Property(e => e.LaborPeriodEndingDate)
            .HasPrecision(6)
            .HasColumnName("VLLDED");
        builder.Property(e => e.Vlldid)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("VLLDID");
        builder.Property(e => e.Vllttp)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLLTTP");
        builder.Property(e => e.Vlmail)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("VLMAIL");
        builder.Property(e => e.EnteredByBusinessUnit)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLMCU");
        builder.Property(e => e.Vlmcuo)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLMCUO");
        builder.Property(e => e.EffectOnGl)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLNMTH");
        builder.Property(e => e.Vlobj)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("VLOBJ");
        builder.Property(e => e.OverTimeCode)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLOHF");
        builder.Property(e => e.Vlopsq)
            .HasColumnType("NUMBER")
            .HasColumnName("VLOPSQ");
        builder.Property(e => e.Vlotrulecd)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLOTRULECD");
        builder.Property(e => e.Vlp001)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLP001");
        builder.Property(e => e.Vlp002)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLP002");
        builder.Property(e => e.Vlp003)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLP003");
        builder.Property(e => e.Vlp004)
            .HasMaxLength(3)
            .IsFixedLength()
            .HasColumnName("VLP004");
        builder.Property(e => e.Vlpalf)
            .HasMaxLength(12)
            .IsFixedLength()
            .HasColumnName("VLPALF");
        builder.Property(e => e.Vlpanp)
            .HasMaxLength(9)
            .IsFixedLength()
            .HasColumnName("VLPANP");
        builder.Property(e => e.Vlpayg)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLPAYG");
        builder.Property(e => e.PayInAdvance)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLPAYLIA");
        builder.Property(e => e.PayMultiplier)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPAYM");
        builder.Property(e => e.Vlpayn)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLPAYN");
        builder.Property(e => e.Vlpb)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLPB");
        builder.Property(e => e.Vlpbrt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPBRT");
        builder.Property(e => e.PrintMethod)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLPCK");
        builder.Property(e => e.Vlpcun)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPCUN");
        builder.Property(e => e.DbaCode)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPDBA");
        builder.Property(e => e.PayFrequency)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLPFRQ");
        builder.Property(e => e.Vlpgrp)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLPGRP");
        builder.Property(e => e.Vlphrt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPHRT");
        builder.Property(e => e.HoursWorked)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPHRW");
        builder.Property(e => e.ProgramId)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("VLPID");
        builder.Property(e => e.Vlpn)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPN");
        builder.Property(e => e.Vlpos)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("VLPOS");
        builder.Property(e => e.PayPeriodEndingDate)
            .HasPrecision(6)
            .HasColumnName("VLPPED");
        builder.Property(e => e.Vlppp)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLPPP");
        builder.Property(e => e.Vlpprt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPPRT");
        builder.Property(e => e.PayrollTransactionNumber)
            .HasColumnType("NUMBER")
            .HasColumnName("VLPRTR");
        builder.Property(e => e.EquipmentObjectAccount)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("VLQOBJ");
        builder.Property(e => e.RecordType)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLRCCD");
        builder.Property(e => e.Vlrchgamt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLRCHGAMT");
        builder.Property(e => e.Vlrchgmode)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLRCHGMODE");
        builder.Property(e => e.Vlrco)
            .HasMaxLength(5)
            .IsFixedLength()
            .HasColumnName("VLRCO");
        builder.Property(e => e.Vlrcpy)
            .HasColumnType("NUMBER")
            .HasColumnName("VLRCPY");
        builder.Property(e => e.Vlrilt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLRILT");
        builder.Property(e => e.Vlrkid)
            .HasColumnType("NUMBER")
            .HasColumnName("VLRKID");
        builder.Property(e => e.Vlrtwc)
            .HasColumnType("NUMBER")
            .HasColumnName("VLRTWC");
        builder.Property(e => e.Vlsaly)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLSALY");
        builder.Property(e => e.Vlsamt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLSAMT");
        builder.Property(e => e.GlSubledger)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("VLSBL");
        builder.Property(e => e.SubledgerType)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLSBLT");
        builder.Property(e => e.SecurityIndicator)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLSEC$");
        builder.Property(e => e.IsVoided)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLSFLG");
        builder.Property(e => e.Vlshd)
            .HasColumnType("NUMBER")
            .HasColumnName("VLSHD");
        builder.Property(e => e.ShiftCode)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLSHFT");
        builder.Property(e => e.HourlyRate)
            .HasColumnType("NUMBER")
            .HasColumnName("VLSHRT");
        builder.Property(e => e.StatusFlag)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLSSFL");
        builder.Property(e => e.SelfServiceRecordId)
            .HasMaxLength(9)
            .IsFixedLength()
            .HasColumnName("VLSSREID");
        builder.Property(e => e.Vlstip)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLSTIP");
        builder.Property(e => e.SubsidiaryId)
            .HasMaxLength(8)
            .IsFixedLength()
            .HasColumnName("VLSUB");
        builder.Property(e => e.AccountNumberTimecardOverride)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCANI");
        builder.Property(e => e.Vltcbr)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCBR");
        builder.Property(e => e.Vltcde)
            .HasColumnType("NUMBER")
            .HasColumnName("VLTCDE");
        builder.Property(e => e.FromDate)
            .HasPrecision(6)
            .HasColumnName("VLTCFD");
        builder.Property(e => e.HomeBusinessUnitTimecardOverride)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCHB");
        builder.Property(e => e.HomeCompanyTimecardOverride)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCHC");
        builder.Property(e => e.BusinessUnitChargeoutTimecardOverride)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCJL");
        builder.Property(e => e.JobStepTimecardOverride)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCJS");
        builder.Property(e => e.JobTypeTimecardOverride)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCJT");
        builder.Property(e => e.Vltcpc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCPC");
        builder.Property(e => e.Vltcpf)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCPF");
        builder.Property(e => e.Vltcrt)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCRT");
        builder.Property(e => e.Vltcsc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCSC");
        builder.Property(e => e.Vltcsd)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCSD");
        builder.Property(e => e.ThruDate)
            .HasPrecision(6)
            .HasColumnName("VLTCTD");
        builder.Property(e => e.Vltcun)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCUN");
        builder.Property(e => e.Vltcwc)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCWC");
        builder.Property(e => e.Vltcws)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLTCWS");
        builder.Property(e => e.Vltskid)
            .HasColumnType("NUMBER")
            .HasColumnName("VLTSKID");
        builder.Property(e => e.Vltt01)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT01");
        builder.Property(e => e.Vltt02)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT02");
        builder.Property(e => e.Vltt03)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT03");
        builder.Property(e => e.Vltt04)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT04");
        builder.Property(e => e.Vltt05)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT05");
        builder.Property(e => e.Vltt06)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT06");
        builder.Property(e => e.Vltt07)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT07");
        builder.Property(e => e.Vltt08)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT08");
        builder.Property(e => e.Vltt09)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT09");
        builder.Property(e => e.Vltt10)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT10");
        builder.Property(e => e.Vltt11)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT11");
        builder.Property(e => e.Vltt12)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT12");
        builder.Property(e => e.Vltt13)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT13");
        builder.Property(e => e.Vltt14)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT14");
        builder.Property(e => e.Vltt15)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLTT15");
        builder.Property(e => e.Vluamt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLUAMT");
        builder.Property(e => e.Vlum)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLUM");
        builder.Property(e => e.UnionCode)
            .HasMaxLength(6)
            .IsFixedLength()
            .HasColumnName("VLUN");
        builder.Property(e => e.DateUpdated)
            .HasPrecision(6)
            .HasColumnName("VLUPMJ");
        builder.Property(e => e.LastUpdateTime)
            .HasColumnType("NUMBER")
            .HasColumnName("VLUPMT");
        builder.Property(e => e.Vluser)
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("VLUSER");
        builder.Property(e => e.Vlusr)
            .HasMaxLength(18)
            .IsFixedLength()
            .HasColumnName("VLUSR");
        builder.Property(e => e.Vlwcam)
            .HasColumnType("NUMBER")
            .HasColumnName("VLWCAM");
        builder.Property(e => e.Vlwcmb)
            .HasColumnType("NUMBER")
            .HasColumnName("VLWCMB");
        builder.Property(e => e.Vlwcmo)
            .HasColumnType("NUMBER")
            .HasColumnName("VLWCMO");
        builder.Property(e => e.WorkersCompInsuranceCode)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("VLWCMP");
        builder.Property(e => e.Vlwcmx)
            .HasColumnType("NUMBER")
            .HasColumnName("VLWCMX");
        builder.Property(e => e.Vlwcnt)
            .HasColumnType("NUMBER")
            .HasColumnName("VLWCNT");
        builder.Property(e => e.Vlwcty)
            .HasColumnType("NUMBER")
            .HasColumnName("VLWCTY");
        builder.Property(e => e.Vlwet)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLWET");
        builder.Property(e => e.Vlwr01)
            .HasMaxLength(4)
            .IsFixedLength()
            .HasColumnName("VLWR01");
        builder.Property(e => e.Vlws)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLWS");
        builder.Property(e => e.Vlwst)
            .HasColumnType("NUMBER")
            .HasColumnName("VLWST");
        builder.Property(e => e.Vlyst)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLYST");
        builder.Property(e => e.Vlyst1)
            .HasMaxLength(1)
            .IsFixedLength()
            .HasColumnName("VLYST1");
        builder.Property(e => e.Vlz1cm)
            .HasMaxLength(100)
            .IsFixedLength()
            .HasColumnName("VLZ1CM");
        builder.Property(e => e.Vlz1cr)
            .HasMaxLength(2)
            .IsFixedLength()
            .HasColumnName("VLZ1CR");
    }
}
