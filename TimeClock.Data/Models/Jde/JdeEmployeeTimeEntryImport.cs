using System.ComponentModel.DataAnnotations;

namespace TimeClock.Data.Models.Jde;

/// <summary>
/// F06116Z1 | Employee Transactions - Batch File
/// </summary>
public partial class JdeEmployeeTimeEntryImport : IJdeEntityModel
{
    [Required]
    /// <summary>
    /// VLEDUS | EDI - User ID
    /// </summary>
    /// <remarks>EDI ID of user making time entry</remarks>
    public string EdiId { get; set; } = null!;
    [Required]
    /// <summary>
    /// VLEDTN | EDI - Transaction Number
    /// </summary>
    public string EdiTransactionNumber { get; set; } = null!;
    [Required]
    /// <summary>
    /// VLEDLN | EDI - Line Number
    /// </summary>
    public decimal EdiLineNumber { get; set; }
    [Required]
    /// <summary>
    /// VLEDDT | EDI - Transmission Date
    /// </summary>
    public int? EdiSubmitDate { get; set; }
    /// <summary>
    /// VLEDER | EDI - Send/Receive Indicator
    /// </summary>
    /// <remarks>Single space or B (both)</remarks>
    public string? EdiSendReceive { get; set; }
    /// <summary>
    /// VLEDSP | EDI - Successfully Processed
    /// </summary>
    /// <remarks>bool - 0 or 1</remarks>
    public string? IsEdiSuccessfullyProcessed { get; set; } = "0";
    [Required]
    /// <summary>
    /// VLEDBT | EDI - Batch Number
    /// </summary>
    public string EdiBatchNumber { get; set; } = null!;
    [Required]
    /// <summary>
    /// VLAN8 | Address Number
    /// </summary>
    /// <remarks>AN8/User ID for user whose time entry is being submitted</remarks>
    public int? EmployeeId { get; set; }
    [Required]
    /// <summary>
    /// VLPRTR | Transaction No. - Payroll
    /// </summary>
    /// <remarks>JDE Next NUmber -> x0010GetNextNumber() business function to get payroll transaction number -> system code 06, next numbering index 3, company 00001</remarks>
    public int? PayrollTransactionNumber { get; set; }
    /// <summary>
    /// VLRCCD | Record Type
    /// </summary>
    /// <remarks>EITHER " " or "1" (Payroll processing only)</remarks>
    public string? RecordType { get; set; } = "1"; // per jeff
    /// <summary>
    /// VLAM | Account Mode - G/L
    /// </summary>
    /// <remarks>EITHER 1 (Short ID) or 2 (Long ID)</remarks>
    public string? GlAccountMode { get; set; }
    /// <summary>
    /// VLCO | Company
    /// </summary>
    public string? Company { get; set; }
    /// <summary>
    /// VLHMCO | Company - Home
    /// </summary>
    /// <remarks>Company ID of user whose time entry is being represented</remarks>
    public string? CompanyHome { get; set; }
    /// <summary>
    /// VLHMCU | Business Unit - Home
    /// </summary>
    /// <remarks>YAHMCU of user whose time entry is represented</remarks>
    public string? UserBusinessUnit { get; set; }
    /// <summary>
    /// VLMCU | Business Unit
    /// </summary>
    /// <remarks>YAHMCU of user who created the time entry</remarks>
    public string? EnteredByBusinessUnit { get; set; }
    /// <summary>
    /// VLOBJ | Object Account
    /// </summary>
    public string? Vlobj { get; set; }
    /// <summary>
    /// VLSUB | Subsidiary
    /// </summary>
    public string? SubsidiaryId { get; set; }
    /// <summary>
    /// VLSBL | Subledger - G/L
    /// </summary>
    public string? GlSubledger { get; set; }
    /// <summary>
    /// VLSBLT | Subledger Type
    /// </summary>
    /// <remarks>EITHER I (Item Number (Short)), W (Work Order Number), M (Summarized Work Order Number), " " (null)</remarks>
    public string? SubledgerType { get; set; }
    /// <summary>
    /// VLMCUO | Business Unit - Chargeout
    /// </summary>
    /// <remarks>Use Business Unit Number for location</remarks>
    public string? Vlmcuo { get; set; }
    /// <summary>
    /// VLMAIL | Routing Code - Check
    /// </summary>
    /// <remarks></remarks>
    public string? Vlmail { get; set; }
    [Required]
    /// <summary>
    /// VLPHRW | Hours Worked
    /// </summary>
    /// <remarks>Tally of all hours ever worked. All time records should reflect the current tally.</remarks>
    public decimal? HoursWorked { get; set; }
    /// <summary>
    /// VLGPA | Amount - Gross Pay
    /// </summary>
    /// <remarks></remarks>
    public decimal? GrossPay { get; set; }
    /// <summary>
    /// VLDPA | Amount - Distributed Gross Pay
    /// </summary>
    public decimal? DistributedGrossPay { get; set; }
    /// <summary>
    /// VLUN | Union Code
    /// </summary>
    public string? UnionCode { get; set; }
    /// <summary>
    /// VLJBCD | Job Type (Craft) Code
    /// </summary>
    public string? JobType { get; set; }
    /// <summary>
    /// VLJBST | Job Step
    /// </summary>
    public string? JobStep { get; set; }
    /// <summary>
    /// VLWCMP | Worker's Comp. Insurance Code
    /// </summary>
    public string? WorkersCompInsuranceCode { get; set; } = " ";
    [Required]
    /// <summary>
    /// VLPDBA | DBA Code
    /// </summary>
    /// <remarks>
    /// Straight time = 1, Overtime = 100 -  Time Entry Type
    /// (Pay Type Code) Enter a pay type. The pay types are controlled by the F069116 table.
    /// </remarks>
    public int? DbaCode { get; set; }
#warning check time entry code
    /// <summary>
    /// VLDEDM | Method of Calculation
    /// </summary>
    /// <remarks>EITHER "%" (Percent Based on Gross) or " "</remarks>
    public string? Vldedm { get; set; }
    /// <summary>
    /// VLPAYM | Multiplier - Pay Type Multiplier
    /// </summary>
    /// <remarks>EITHER 0, 100, 150, 200</remarks>
    public decimal? PayMultiplier { get; set; }
    /// <summary>
    /// VLSHFT | Shift Code
    /// </summary>
    /// <remarks>EITHER " " (null), "2" (Second Shift) or "E" (ESS Time Entry)</remarks>
    public string? ShiftCode { get; set; }
    /// <summary>
    /// VLNMTH | Effect on GL
    /// </summary>
    /// <remarks>EITHER " " or "N"</remarks>
    public string? EffectOnGl { get; set; } = " ";
    /// <summary>
    /// VLPFRQ | Pay Frequency
    /// </summary>
    /// <remarks>EITHER " " (null), "M" (Monthly), "W" (Weekly)</remarks>
    public string? PayFrequency { get; set; }
    [Required]
    /// <summary>
    /// VLDWK | Date - Worked
    /// </summary>
    public int? DateWorked { get; set; }
    /// <summary>
    /// VLDW | Day of the Week
    /// </summary>
    /// <remarks>EITHER " " (null) or "1" (Monday)</remarks>
    public string? DayOfTheWeek { get; set; }
    /// <summary>
    /// VLPPP | Pay Period of the Month
    /// </summary>
    /// <remarks>EITHER " " (null) or "1" (Pay Period 1)</remarks>
    public string? Vlppp { get; set; }
    /// <summary>
    /// VLEQWO | Equipment Worked On
    /// </summary>
    public string? EquipmentWorkedOnId { get; set; } = " ";
    /// <summary>
    /// VLEQCG | Equipment Worked
    /// </summary>
    /// <remarks>Equipment Number</remarks>
    public string? EquipmentUsedId { get; set; }
    /// <summary>
    /// VLQOBJ | Equipment Object Account
    /// </summary>
    public string? EquipmentObjectAccount { get; set; }
    /// <summary>
    /// VLEQHR | Hours - Equipment
    /// </summary>
    /// <remarks>Hours in hundreds?? (800, 600, 2200, 50)</remarks>
    public decimal? EquipmentHours { get; set; }
    /// <summary>
    /// VLEXR | Name - Remark Explanation
    /// </summary>
    /// <remarks>Time entry type (Regular, ETO) padded right to 30</remarks>
    public string? TimeEntryTypeDescription { get; set; }
    /// <summary>
    /// VLP002 | Category Codes - Payroll2
    /// </summary>
    /// <remarks>EITHER "   ", "B  ", "Y  " - padded right 3</remarks>
    public string? Vlp002 { get; set; }
    /// <summary>
    /// VLP004 | Category Codes - Payroll4
    /// </summary>
    /// <remarks>EITHER " " (null), "Y" (Email Deposit Advices), "N" (Do Not Email Deposit Advices) - padded right 3</remarks>
    public string? Vlp004 { get; set; }
    [Required]
    /// <summary>
    /// VLUSER | User ID
    /// </summary>
    /// <remarks>Domain User Name</remarks>
    public string? Vluser { get; set; }
    /// <summary>
    /// VLICU | Batch Number
    /// </summary>
    /// <remarks>Entered as 0. JDE assigns value after processing</remarks>
    public int? BatchNumber { get; set; } = 0;
    [Required]
    /// <summary>
    /// VLDICJ | Date - Batch (Julian)
    /// </summary>
    public int? BatchDate { get; set; }
    [Required]
    /// <summary>
    /// VLUPMJ | Date - Updated
    /// </summary>
    /// <remarks>Date created OR updated</remarks>
    public int? DateUpdated { get; set; }
    [Required]
    /// <summary>
    /// VLPID | Program ID
    /// </summary>
    public string? ProgramId { get; set; }
    [Required]
    /// <summary>
    /// VLANI | Account Number - Input (Mode Unknown)
    /// </summary>
    /// <remarks>Full GL Account eg: 2012301.6011.010180 (Business Unit.Pay Type Obj.Sub Account)</remarks>
    public string? GlAccount { get; set; }
    /// <summary>
    /// VLCTRY | Century
    /// </summary>
    /// <remarks>Should be set to first 2 digits of year</remarks>
    public int? Century { get; set; } = 20;
    /// <summary>
    /// VLSFLG | Void Flag (Y/N)
    /// </summary>
    /// <remarks>EITHER " " or "N"</remarks>
    public string? IsVoided { get; set; } = " ";
    /// <summary>
    /// VLPCK | Method of Printing
    /// </summary>
    /// <remarks>EITHER " " (null) or "I" (Print individual transactions)</remarks>
    public string? PrintMethod { get; set; }
    /// <summary>
    /// VLCMTH | Shift Diff Calc Sequence
    /// </summary>
    /// <remarks>EITHER " " (null) or "1" ((rate+shift diff)x(multi) xhrs))</remarks>
    public string? ShiftDiffCalcSequence { get; set; }
    /// <summary>
    /// VLACO | Available DBA
    /// </summary>
    /// <remarks>EITHER " " or "N"</remarks>
    public string? DbaAvailable { get; set; }
    /// <summary>
    /// VLAI | Type - Sales
    /// </summary>
    /// <remarks>EITHER " " or "C"</remarks>
    public string? Vlai { get; set; }
    /// <summary>
    /// VLSEC$ | Security Indicator
    /// </summary>
    /// <remarks>EITHER "U", "E", " "</remarks>
    public string? SecurityIndicator { get; set; }
    /// <summary>
    /// VLOHF | Overtime Code
    /// </summary>
    /// <remarks>EITHER "B" (Double time), "R" (Regular overtime), "A" (Overtime), "D" (Holiday), " " (null)</remarks>
    public string? OverTimeCode { get; set; }
    /// <summary>
    /// VLDEP1 | Deduction Period 1
    /// </summary>
    /// <remarks>EITHER "Y" (Take during cur. period.) or " " (null)</remarks>
    public string? Vldep1 { get; set; }
    /// <summary>
    /// VLDEP2 | Deduction Period 2
    /// </summary>
    /// <remarks>EITHER "Y" (Take during cur. period.) or " " (null)</remarks>
    public string? Vldep2 { get; set; }
    /// <summary>
    /// VLDEP3 | Deduction Period 3
    /// </summary>
    /// <remarks>EITHER "Y" (Take during cur. period.) or " " (null)</remarks>
    public string? Vldep3 { get; set; }
    /// <summary>
    /// VLDEP4 | Deduction Period 4
    /// </summary>
    /// <remarks>EITHER "Y" (Take during cur. period.) or " " (null)</remarks>
    public string? Vldep4 { get; set; }
    /// <summary>
    /// VLDEP5 | Deduction Period 5
    /// </summary>
    /// <remarks>EITHER "Y" (Take during cur. period.) or " " (null)</remarks>
    public string? Vldep5 { get; set; }
    /// <summary>
    /// VLSTIP | Batch Timecard Offsite Flag
    /// </summary>
    /// <remarks>EITHER "0", "1", " "</remarks>
    public string? Vlstip { get; set; } = " ";
    /// <summary>
    /// VLALPH | Name - Alpha
    /// </summary>
    public string? EmployeeName { get; set; }
    /// <summary>
    /// VLIIAP | Auto Pay Methods
    /// </summary>
    /// <remarks>EITHER "Y" ($ part of employee's base pay) or " " (null)</remarks>
    public string? AutoPayMethods { get; set; } = " ";
    /// <summary>
    /// VLSHRT | Rate - Hourly
    /// </summary>
    public decimal? HourlyRate { get; set; } = 0;
    /// <summary>
    /// VLLD | Percent or Amount
    /// </summary>
    /// <remarks>EITHER "%" (Amt is % of hr rte/add 2 hr rt), "H" (Amt in shft dif fld add 2 hr r), " " (null)</remarks>
    public string? Vlld { get; set; } = " ";
    /// <summary>
    /// VLSSFL | Time Entry Status Flag
    /// </summary>
    public string? StatusFlag { get; set; } = " ";
    /// <summary>
    /// VLRKID | Leave Request Number
    /// </summary>
    public int? Vlrkid { get; set; } = 0;
    /// <summary>
    /// VLUPMT | Time - Last Updated
    /// </summary>
    public int? LastUpdateTime { get; set; } = 0;
    /// <summary>
    /// VLANPA | Supervisor
    /// </summary>
    /// <remarks>Supervisor AN8</remarks>
    public int? SupervisorId { get; set; } = 0;
    /// <summary>
    /// VLSSREID | Self Service Record Identifier
    /// </summary>
    public string? SelfServiceRecordId { get; set; } = " ";
    /// <summary>
    /// VLPAYLIA | Pay In Advance
    /// </summary>
    /// <remarks>EITHER "0" or "N"</remarks>
    public string? PayInAdvance { get; set; }
    /// <summary>
    /// VLTCFD | Timecard From Date
    /// </summary>
    /// <remarks>Usually 0</remarks>
    public int? FromDate { get; set; } = 0;
    /// <summary>
    /// VLTCTD | Timecard Thru Date
    /// </summary>
    /// <remarks>Usually 0</remarks>
    public int? ThruDate { get; set; } = 0;
    /// <summary>
    /// VLTCHC | Home Company Timecard Override
    /// </summary>
    /// <remarks>EITHER "1" or " ", usually blank</remarks>
    public string? HomeCompanyTimecardOverride { get; set; } = " ";
    /// <summary>
    /// VLTCHB | Home Business Unit Timecard Override
    /// </summary>
    /// <remarks>EITHER "1" or " "</remarks>
    public string? HomeBusinessUnitTimecardOverride { get; set; } = " ";
    /// <summary>
    /// VLTCJT | Job Type Timecard Override
    /// </summary>
    /// <remarks>EITHER "1" or " "</remarks>
    public string? JobTypeTimecardOverride { get; set; } = " ";
    /// <summary>
    /// VLTCJS | Job Step Timecard Override
    /// </summary>
    /// <remarks>EITHER "1" or " "</remarks>
    public string? JobStepTimecardOverride { get; set; } = " ";
    /// <summary>
    /// VLTCJL | Business Unit Charge out Timecard Override
    /// </summary>
    /// <remarks></remarks>
    /// <remarks>EITHER "1" or " "</remarks>
    public string? BusinessUnitChargeoutTimecardOverride { get; set; } = " ";
    /// <summary>
    /// VLTCANI | Account Number Timecard Override
    /// </summary>
    /// <remarks>EITHER "1" or " "</remarks>
    public string? AccountNumberTimecardOverride { get; set; } = " ";
    /// <summary>
    /// VLLDED | Labor Period Ending Date
    /// </summary>
    /// <remarks>Currently the Saturday following the entry date</remarks>
    public int? LaborPeriodEndingDate { get; set; }
    /// <summary>
    /// VLLDID | Labor Distribution Period ID
    /// </summary>
    /// <remarks>Values such as: WK55 (Full Time - 55), WK325 (Part Time - 32.50), WK475 (Full Time - 47.50), " " (null)</remarks>
    public string? Vlldid { get; set; } = " ";




    #region Not Used
    /// <summary>
    /// VLSALY | Pay Class (H/S/P)
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlsaly { get; set; } = " ";
    /// <summary>
    /// VLRCPY | Recharge Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlrcpy { get; set; } = 0;
    /// <summary>
    /// VLP001 | Category Codes - Payroll1
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlp001 { get; set; } = " ";
    /// <summary>
    /// VLEDTY | Type Record
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vledty { get; set; } = " ";
    /// <summary>
    /// VLEDCT | EDI - Document Type
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vledct { get; set; } = " ";
    /// <summary>
    /// VLEDTS | EDI - Transaction Set Number
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vledts { get; set; } = " ";
    /// <summary>
    /// VLEDFT | EDI - Translation Format
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vledft { get; set; } = " ";
    /// <summary>
    /// VLEDSQ | Record Sequence
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vledsq { get; set; } = 0;
    [Required]
    /// <summary>
    /// VLEDDL | EDI - Detail Lines Processed
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vleddl { get; set; } = 0;
    [Required]
    /// <summary>
    /// VLEDTC | EDI - Transaction Action
    /// </summary>
    /// <remarks>ALWAYS A (Add)</remarks>
    public string? Vledtc { get; set; } = "A";
    [Required]
    /// <summary>
    /// VLEDTR | EDI - Transaction Type
    /// </summary>
    /// <remarks>ALWAYS 1</remarks>
    public string? Vledtr { get; set; } = "1";
    /// <summary>
    /// VLEDGL | Batch File Create G/L Record
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vledgl { get; set; } = " ";
    /// <summary>
    /// VLPANP | Payroll ID - Input (Mode Unknown)
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlpanp { get; set; } = " ";
    /// <summary>
    /// VLPALF | Name - Alpha Sort
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlpalf { get; set; } = " ";
    /// <summary>
    /// VLCKCN | Check Control Number
    /// </summary>
    /// <remarks>ALWATS 0</remarks>
    public int? Vlckcn { get; set; } = 0;
    /// <summary>
    /// VLRCO | Company _ Recharge
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlrco { get; set; } = " ";
    /// <summary>
    /// VLGMCU | Business Unit - Recharge
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlgmcu { get; set; } = " ";
    /// <summary>
    /// VLGOBJ | Object Account - Recharge
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlgobj { get; set; } = " ";
    /// <summary>
    /// VLGSUB | Subsidiary - Recharge
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlgsub { get; set; } = " ";
    /// <summary>
    /// VLWR01 | Categories - Work Order 01
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlwr01 { get; set; } = " ";
    /// <summary>
    /// VLOPSQ | Sequence Number - Operations
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlopsq { get; set; } = 0;
    /// <summary>
    /// VLRILT | Labor Type - Routing Instructions
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlrilt { get; set; } = " ";
    /// <summary>
    /// VLBDSN | Serial Number - Bond
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlbdsn { get; set; } = " ";
    /// <summary>
    /// VLPCUN | Units - Pieces
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlpcun { get; set; } = 0;
    /// <summary>
    /// VLUM | Unit of Measure
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlum { get; set; } = " ";
    /// <summary>
    /// VLPHRT | Rate - Hourly
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlphrt { get; set; } = 0;
    /// <summary>
    /// VLPPRT | Rate - Piecework
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlpprt { get; set; } = 0;
    /// <summary>
    /// VLBHRT | Rate - Base Hourly
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlbhrt { get; set; } = 0;
    /// <summary>
    /// VLPBRT | Rate - Distribution (or Billing)
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlpbrt { get; set; } = 0;
    /// <summary>
    /// VLBDRT | Rate - Recharge Burden
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlbdrt { get; set; } = 0;
    /// <summary>
    /// VLSAMT | Amount - Sales Generated
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public decimal? Vlsamt { get; set; } = 0;
    /// <summary>
    /// VLWST | Work State
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlwst { get; set; } = 0;
    /// <summary>
    /// VLWCNT | Work County
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlwcnt { get; set; } = 0;
    /// <summary>
    /// VLWCTY | Work City
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlwcty { get; set; } = 0;
    /// <summary>
    /// VLWET | Sub Class - Workers Comp
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlwet { get; set; } = " ";
    /// <summary>
    /// VLGENA | General Liability Premium Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlgena { get; set; } = 0;
    /// <summary>
    /// VLWCAM | Workers Comp Premium Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlwcam { get; set; } = 0;
    /// <summary>
    /// VLWCMB | Workers Comp Premium Base
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlwcmb { get; set; } = 0;
    /// <summary>
    /// VLGENB | General Liability Premium Base
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlgenb { get; set; } = 0;
    /// <summary>
    /// VLWCMO | Workers Comp Overtime Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlwcmo { get; set; } = 0;
    /// <summary>
    /// VLGENO | General Liability Overtime Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlgeno { get; set; } = 0;
    /// <summary>
    /// VLWCMX | Workers Comp Excludable Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlwcmx { get; set; } = 0;
    /// <summary>
    /// VLGENX | General Liability Excludable Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlgenx { get; set; } = 0;
    /// <summary>
    /// VLHMO | Month - Update of History
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlhmo { get; set; } = 0;
    /// <summary>
    /// VLPB | Source of Pay
    /// </summary>
    /// <remarks>ALWAYS H</remarks>
    public string? Vlpb { get; set; } = "H";
    /// <summary>
    /// VLSHD | Amount - Shift Differential
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlshd { get; set; } = 0;
    /// <summary>
    /// VLFY | Fiscal Year
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlfy { get; set; } = 0;
    /// <summary>
    /// VLDGL | Date - For G/L (and Voucher)
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vldgl { get; set; } = 0;
    /// <summary>
    /// VLPN | Period Number - General Ledger
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlpn { get; set; } = 0;
    /// <summary>
    /// VLPPED | Date - Pay Period Ending
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? PayPeriodEndingDate { get; set; } = 0;
    /// <summary>
    /// VLDTBT | Date - Time Clock Start Date and Time
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vldtbt { get; set; } = 0;
    /// <summary>
    /// VLTCDE | Date Time Clock End
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vltcde { get; set; } = 0;
    /// <summary>
    /// VLEQCO | Company - Equipment
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vleqco { get; set; } = " ";
    /// <summary>
    /// VLERC | Equipment Rate Code
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlerc { get; set; } = " ";
    /// <summary>
    /// VLEQRT | Billing Rate - Equipment
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vleqrt { get; set; } = 0;
    /// <summary>
    /// VLEQGR | Amount _ Equipment Gross
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vleqgr { get; set; } = 0;
    /// <summary>
    /// VLP003 | Category Codes - Payroll3
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlp003 { get; set; } = " ";
    /// <summary>
    /// VLCMMT | Check Comment (Y/N)
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlcmmt { get; set; } = " ";
    /// <summary>
    /// VLCKDT | Date - Pay Check
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlckdt { get; set; } = 0;
    /// <summary>
    /// VLUAMT | Amount - Uprate
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vluamt { get; set; } = 0;
    /// <summary>
    /// VLYST | Processed Code
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlyst { get; set; } = " ";
    /// <summary>
    /// VLGICU | General Ledger Batch Number
    /// </summary>
    /// <remarks>Always 0</remarks>
    public int? Vlgicu { get; set; } = 0;
    /// <summary>
    /// VLANN8 | Address Number-Provider/Trustee
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlann8 { get; set; } = 0;
    /// <summary>
    /// VLPGRP | Deduct/Benefit Override Code
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlpgrp { get; set; } = " ";
    /// <summary>
    /// VLPAYG | Effect on Gross Pay
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlpayg { get; set; } = " ";
    /// <summary>
    /// VLPAYN | Effect on Net Pay
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlpayn { get; set; } = " ";
    /// <summary>
    /// VLWS | Work Schedule Code
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlws { get; set; } = " ";
    /// <summary>
    /// VLICC | Interim Check Code
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlicc { get; set; } = " ";
    /// <summary>
    /// VLICS | Interim Check Status
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlics { get; set; } = " ";
    /// <summary>
    /// VLTT01 | Non-Taxable Authority Types 01
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt01 { get; set; } = " ";
    /// <summary>
    /// VLTT02 | Non-Taxable Authority Types 02
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt02 { get; set; } = " ";
    /// <summary>
    /// VLTT03 | Non-Taxable Authority Types 03
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt03 { get; set; } = " ";
    /// <summary>
    /// VLTT04 | Non-Taxable Authority Types 04
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt04 { get; set; } = " ";
    /// <summary>
    /// VLTT05 | Non-Taxable Authority Types 05
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt05 { get; set; } = " ";
    /// <summary>
    /// VLTT06 | Non-Taxable Authority Types 06
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt06 { get; set; } = " ";
    /// <summary>
    /// VLTT07 | Non-Taxable Authority Types 07
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt07 { get; set; } = " ";
    /// <summary>
    /// VLTT08 | Non-Taxable Authority Types 08
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt08 { get; set; } = " ";
    /// <summary>
    /// VLTT09 | Non-Taxable Authority Types 09
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt09 { get; set; } = " ";
    /// <summary>
    /// VLTT10 | Non-Taxable Authority Types 10
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt10 { get; set; } = " ";
    /// <summary>
    /// VLTT11 | Non-Taxable Authority Types 11
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt11 { get; set; } = " ";
    /// <summary>
    /// VLTT12 | Non-Taxable Authority Types 12
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt12 { get; set; } = " ";
    /// <summary>
    /// VLTT13 | Non-Taxable Authority Types 13
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt13 { get; set; } = " ";
    /// <summary>
    /// VLTT14 | Non-Taxable Authority Types 14
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt14 { get; set; } = " ";
    /// <summary>
    /// VLTT15 | Non-Taxable Authority Types 15
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltt15 { get; set; } = " ";
    /// <summary>
    /// VLUSR | Payroll Lockout Identification
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlusr { get; set; } = " ";
    /// <summary>
    /// VLEPA | Amount - Entered Gross Pay
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlepa { get; set; } = 0;
    /// <summary>
    /// VLBFA | Benefit Factor
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlbfa { get; set; } = 0;
    /// <summary>
    /// VLRTWC | Rate - Workers Comp Insurance
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlrtwc { get; set; } = 0;
    /// <summary>
    /// VLGENR | Rate - Workers Comp Gen Liability
    /// </summary>
    /// <remarks>AWLAYS 0</remarks>
    public decimal? Vlgenr { get; set; } = 0;
    /// <summary>
    /// VLADV | Advance on Pay
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vladv { get; set; } = " ";
    /// <summary>
    /// VLFICM | Tax Calc Method
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlficm { get; set; } = " ";
    /// <summary>
    /// VLDTAB | Table Code
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vldtab { get; set; } = " ";
    /// <summary>
    /// VLDTSP | Date - Time Stamp
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vldtsp { get; set; } = 0;
    /// <summary>
    /// VLYST1 | Status- Payroll 01
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlyst1 { get; set; } = " ";
    /// <summary>
    /// VLACTB | Activity-Based Costing Activity Code
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlactb { get; set; } = " ";
    /// <summary>
    /// VLABR1 | Managerial Analysis Code 1
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabr1 { get; set; } = " ";
    /// <summary>
    /// VLABT1 | Managerial Analysis Type 1
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabt1 { get; set; } = " ";
    /// <summary>
    /// VLABR2 | Managerial Analysis Code 2
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabr2 { get; set; } = " ";
    /// <summary>
    /// VLABT2 | Managerial Analysis Type 2
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabt2 { get; set; } = " ";
    /// <summary>
    /// VLABR3 | Managerial Analysis Code 3
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabr3 { get; set; } = " ";
    /// <summary>
    /// VLABT3 | Managerial Analysis Type 3
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabt3 { get; set; } = " ";
    /// <summary>
    /// VLABR4 | Managerial Analysis Code 4
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabr4 { get; set; } = " ";
    /// <summary>
    /// VLABT4 | Managerial Analysis Type 4
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlabt4 { get; set; } = " ";
    /// <summary>
    /// VLITM | Item Number - Short
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlitm { get; set; } = 0;
    /// <summary>
    /// VLBLGRT | Rate - Billing Rate
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlblgrt { get; set; } = 0;
    /// <summary>
    /// VLRCHGAMT | Rate - Recharge Amount
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlrchgamt { get; set; } = 0;
    /// <summary>
    /// VLFBLGRT | Rate - Billing Foreign
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlfblgrt { get; set; } = 0;
    /// <summary>
    /// VLFRCHGAMT | Rate - Recharge Amount Foreign
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlfrchgamt { get; set; } = 0;
    /// <summary>
    /// VLCRR | Currency Conversion Rate - Spot Rate
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlcrr { get; set; } = 0;
    /// <summary>
    /// VLCRCD | Currency Code - From
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlcrcd { get; set; } = " ";
    /// <summary>
    /// VLCRDC | Currency Code - To
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlcrdc { get; set; } = " ";
    /// <summary>
    /// VLRCHGMODE | Recharge Mode
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlrchgmode { get; set; } = " ";
    /// <summary>
    /// VLLTTP | Leave Type
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vllttp { get; set; } = " ";
    /// <summary>
    /// VLPOS | Position ID
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlpos { get; set; } = " ";
    /// <summary>
    /// VLOTRULECD | Code - Overtime Rule
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlotrulecd { get; set; } = " ";
    /// <summary>
    /// VLTSKID | Task Unique Key ID
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vltskid { get; set; } = 0;
    /// <summary>
    /// VLDOCM | Document - Matching(Payment or Item)
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vldocm { get; set; } = 0;
    /// <summary>
    /// VLHWPD | Hours Worked Per Day
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public decimal? Vlhwpd { get; set; } = 0;
    /// <summary>
    /// VLINSTID | Instance Identifier
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlinstid { get; set; } = " ";
    /// <summary>
    /// VLTCUN | Union Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcun { get; set; } = " ";
    /// <summary>
    /// VLTCRT | Hourly Rate Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcrt { get; set; } = " ";
    /// <summary>
    /// VLTCSC | Shift Code Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcsc { get; set; } = " ";
    /// <summary>
    /// VLTCSD | Shift Differential Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcsd { get; set; } = " ";
    /// <summary>
    /// VLTCBR | Billing Rate Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcbr { get; set; } = " ";
    /// <summary>
    /// VLTCPC | Piece Rate Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcpc { get; set; } = " ";
    /// <summary>
    /// VLTCWC | Workers Comp Code Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcwc { get; set; } = " ";
    /// <summary>
    /// VLTCWS | Workers Comp Sub Class Timecard Override
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcws { get; set; } = " ";
    /// <summary>
    /// VLJBLC | Contract Labor Category
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vljblc { get; set; } = " ";
    /// <summary>
    /// VLTCPF | Timecard Processed Flag
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vltcpf { get; set; } = " ";
    /// <summary>
    /// VLCRFL | Timecard Corrections Flag
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlcrfl { get; set; } = " ";
    /// <summary>
    /// VLCPTR | Correction Timecard Historical Link
    /// </summary>
    /// <remarks>ALWAYS 0</remarks>
    public int? Vlcptr { get; set; } = 0;
    /// <summary>
    /// VLZ1CR | Timecard Change Reason
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlz1cr { get; set; } = " ";
    /// <summary>
    /// VLZ1CM | Change Comments
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlz1cm { get; set; } = " ";
    /// <summary>
    /// VLAUSPTWW | State Payroll Tax State Where Worked
    /// </summary>
    /// <remarks>NOT USED</remarks>
    public string? Vlausptww { get; set; } = " ";

    #endregion Not Used
}
