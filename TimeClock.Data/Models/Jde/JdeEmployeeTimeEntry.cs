namespace TimeClock.Data.Models.Jde;
/// <summary>
/// F06116 | Employee Transaction Detail File
/// </summary>
public partial class JdeEmployeeTimeEntry : IJdeEntityModel
{
    /// <summary>
    /// YTAN8 | Address Number
    /// </summary>
    public int EmployeeId { get; set; }
    /// <summary>
    /// YTPALF | Name - Alpha Sort
    /// </summary>
    public string? Ytpalf { get; set; }
    /// <summary>
    /// YTPRTR | Transaction No. - Payroll
    /// </summary>
    public int Ytprtr { get; set; }
    /// <summary>
    /// YTRCCD | Record Type
    /// </summary>
    public char? Ytrccd { get; set; }
    /// <summary>
    /// YTCKCN | Check Control Number
    /// </summary>
    public int? Ytckcn { get; set; }
    /// <summary>
    /// YTAM | Account Mode - G/L
    /// </summary>
    public char? Ytam { get; set; }
    /// <summary>
    /// YTCO | Company
    /// </summary>
    public string? Ytco { get; set; }
    /// <summary>
    /// YTHMCO | Company - Home
    /// </summary>
    public string? Ythmco { get; set; }
    /// <summary>
    /// YTHMCU | Business Unit - Home
    /// </summary>
    public string? Ythmcu { get; set; }
    /// <summary>
    /// YTMCU | Business Unit
    /// </summary>
    public string? Ytmcu { get; set; }
    /// <summary>
    /// YTOBJ | Object Account
    /// </summary>
    public string? Ytobj { get; set; }
    /// <summary>
    /// YTSUB | Subsidiary
    /// </summary>
    public string? Ytsub { get; set; }
    /// <summary>
    /// YTRCO | Company _ Recharge
    /// </summary>
    public string? Ytrco { get; set; }
    /// <summary>
    /// YTGMCU | Business Unit - Recharge
    /// </summary>
    public string? Ytgmcu { get; set; }
    /// <summary>
    /// YTGOBJ | Object Account - Recharge
    /// </summary>
    public string? Ytgobj { get; set; }
    /// <summary>
    /// YTGSUB | Subsidiary - Recharge
    /// </summary>
    public string? Ytgsub { get; set; }
    /// <summary>
    /// YTSBL | Subledger - G/L
    /// </summary>
    public string? Ytsbl { get; set; }
    /// <summary>
    /// YTSBLT | Subledger Type
    /// </summary>
    public string? Ytsblt { get; set; }
    /// <summary>
    /// YTWR01 | Categories - Work Order 01
    /// </summary>
    public string? Ytwr01 { get; set; }
    /// <summary>
    /// YTMCUO | Business Unit - Charge out
    /// </summary>
    public string? Ytmcuo { get; set; }
    /// <summary>
    /// YTMAIL | Routing Code - Check
    /// </summary>
    public string? Ytmail { get; set; }
    /// <summary>
    /// YTPHRW | Hours Worked
    /// </summary>
    public decimal? Ytphrw { get; set; }
    /// <summary>
    /// YTOPSQ | Sequence Number - Operations
    /// </summary>
    public decimal? Ytopsq { get; set; }
    /// <summary>
    /// YTRILT | Labor Type - Routing Instructions
    /// </summary>
    public string? Ytrilt { get; set; }
    /// <summary>
    /// YTITM | Item Number - Short
    /// </summary>
    public int? Ytitm { get; set; }
    /// <summary>
    /// YTPCUN | Units - Pieces
    /// </summary>
    public decimal? Ytpcun { get; set; }
    /// <summary>
    /// YTUM | Unit of Measure
    /// </summary>
    public string? Ytum { get; set; }
    /// <summary>
    /// YTPHRT | Rate - Hourly
    /// </summary>
    public decimal? Ytphrt { get; set; }
    /// <summary>
    /// YTPPRT | Rate - Piecework
    /// </summary>
    public decimal? Ytpprt { get; set; }
    /// <summary>
    /// YTBHRT | Rate - Base Hourly
    /// </summary>
    public decimal? Ytbhrt { get; set; }
    /// <summary>
    /// YTPBRT | Rate - Distribution (or Billing)
    /// </summary>
    public decimal? Ytpbrt { get; set; }
    /// <summary>
    /// YTBDRT | Rate - Recharge Burden
    /// </summary>
    public decimal? Ytbdrt { get; set; }
    /// <summary>
    /// YTSHRT | Rate - Hourly
    /// </summary>
    public decimal? Ytshrt { get; set; }
    /// <summary>
    /// YTSHFT | Shift Code
    /// </summary>
    public char? Ytshft { get; set; }
    /// <summary>
    /// YTSHD | Amount - Shift Differential
    /// </summary>
    public decimal? Ytshd { get; set; }
    /// <summary>
    /// YTPAYM | Multiplier - Pay Type Multiplier
    /// </summary>
    public decimal? Ytpaym { get; set; }
    /// <summary>
    /// YTLD | Percent or Amount
    /// </summary>
    public char? Ytld { get; set; }
    /// <summary>
    /// YTGPA | Amount - Gross Pay
    /// </summary>
    public decimal? Ytgpa { get; set; }
    /// <summary>
    /// YTDPA | Amount - Distributed Gross Pay
    /// </summary>
    public decimal? Ytdpa { get; set; }
    /// <summary>
    /// YTRCPY | Recharge Amount
    /// </summary>
    public decimal? Ytrcpy { get; set; }
    /// <summary>
    /// YTSAMT | Amount - Sales Generated
    /// </summary>
    public decimal? Ytsamt { get; set; }
    /// <summary>
    /// YTUN | Union Code
    /// </summary>
    public string? Ytun { get; set; }
    /// <summary>
    /// YTJBCD | Job Type (Craft) Code
    /// </summary>
    public string? Ytjbcd { get; set; }
    /// <summary>
    /// YTJBST | Job Step
    /// </summary>
    public string? Ytjbst { get; set; }
    /// <summary>
    /// YTWST | Work State
    /// </summary>
    public int? Ytwst { get; set; }
    /// <summary>
    /// YTWCNT | Work County
    /// </summary>
    public int? Ytwcnt { get; set; }
    /// <summary>
    /// YTWCTY | Work City
    /// </summary>
    public int? Ytwcty { get; set; }
    /// <summary>
    /// YTWCMP | Worker's Comp. Insurance Code
    /// </summary>
    public string? Ytwcmp { get; set; }
    /// <summary>
    /// YTWET | Sub Class - Workers Comp
    /// </summary>
    public string? Ytwet { get; set; }
    /// <summary>
    /// YTGENA | General Liability Premium Amount
    /// </summary>
    public decimal? Ytgena { get; set; }
    /// <summary>
    /// YTWCAM | Workers Comp Premium Amount
    /// </summary>
    public decimal? Ytwcam { get; set; }
    /// <summary>
    /// YTWCMB | Workers Comp Premium Base
    /// </summary>
    public decimal? Ytwcmb { get; set; }
    /// <summary>
    /// YTGENB | General Liability Premium Base
    /// </summary>
    public decimal? Ytgenb { get; set; }
    /// <summary>
    /// YTWCMO | Workers Comp Overtime Amount
    /// </summary>
    public decimal? Ytwcmo { get; set; }
    /// <summary>
    /// YTGENO | General Liability Overtime Amount
    /// </summary>
    public decimal? Ytgeno { get; set; }
    /// <summary>
    /// YTWCMX | Workers Comp Excludable Amount
    /// </summary>
    public decimal? Ytwcmx { get; set; }
    /// <summary>
    /// YTGENX | General Liability Excludable Amount
    /// </summary>
    public decimal? Ytgenx { get; set; }
    /// <summary>
    /// YTWCBN | Workers' Compensation Benefit Amount
    /// </summary>
    public decimal? Ytwcbn { get; set; }
    /// <summary>
    /// YTHMO | Month - Update of History
    /// </summary>
    public int? Ythmo { get; set; }
    /// <summary>
    /// YTPDBA | DBA Code
    /// </summary>
    public int? Ytpdba { get; set; }
    /// <summary>
    /// YTPB | Source of Pay
    /// </summary>
    public char? Ytpb { get; set; }
    /// <summary>
    /// YTDEDM | Method of Calculation
    /// </summary>
    public char? Ytdedm { get; set; }
    /// <summary>
    /// YTSALY | Pay Class (H/S/P)
    /// </summary>
    public char? Ytsaly { get; set; }
    /// <summary>
    /// YTNMTH | Effect on GL
    /// </summary>
    public char? Ytnmth { get; set; }
    /// <summary>
    /// YTPFRQ | Pay Frequency
    /// </summary>
    public char? Ytpfrq { get; set; }
    /// <summary>
    /// YTFY | Fiscal Year
    /// </summary>
    public int? Ytfy { get; set; }
    /// <summary>
    /// YTDGL | Date - For G/L (and Voucher)
    /// </summary>
    public int? Ytdgl { get; set; }
    /// <summary>
    /// YTPN | Period Number - General Ledger
    /// </summary>
    public int? Ytpn { get; set; }
    /// <summary>
    /// YTDWK | Date - Worked
    /// </summary>
    public int Ytdwk { get; set; }
    /// <summary>
    /// YTDW | Day of the Week
    /// </summary>
    public char? Ytdw { get; set; }
    /// <summary>
    /// YTPPED | Date - Pay Period Ending
    /// </summary>
    public int? Ytpped { get; set; }
    /// <summary>
    /// YTPPP | Pay Period of the Month
    /// </summary>
    public char? Ytppp { get; set; }
    /// <summary>
    /// YTDTBT | Date - Time Clock Start Date and Time
    /// </summary>
    public int? Ytdtbt { get; set; }
    /// <summary>
    /// YTTCDE | Date Time Clock End
    /// </summary>
    public int? Yttcde { get; set; }
    /// <summary>
    /// YTEQCO | Company - Equipment
    /// </summary>
    public string? Yteqco { get; set; }
    /// <summary>
    /// YTEQWO | Equipment Worked On
    /// </summary>
    public string? Yteqwo { get; set; }
    /// <summary>
    /// YTEQCG | Equipment Worked
    /// </summary>
    public string? Yteqcg { get; set; }
    /// <summary>
    /// YTQOBJ | Equipment Object Account
    /// </summary>
    public string? Ytqobj { get; set; }
    /// <summary>
    /// YTERC | Equipment Rate Code
    /// </summary>
    public string? Yterc { get; set; }
    /// <summary>
    /// YTEQRT | Billing Rate - Equipment
    /// </summary>
    public decimal? Yteqrt { get; set; }
    /// <summary>
    /// YTEQGR | Amount _ Equipment Gross
    /// </summary>
    public decimal? Yteqgr { get; set; }
    /// <summary>
    /// YTEQHR | Hours - Equipment
    /// </summary>
    public decimal? Yteqhr { get; set; }
    /// <summary>
    /// YTEXR | Name - Remark Explanation
    /// </summary>
    public string? Ytexr { get; set; }
    /// <summary>
    /// YTP001 | Category Codes - Payroll1
    /// </summary>
    public string? Ytp001 { get; set; }
    /// <summary>
    /// YTP002 | Category Codes - Payroll2
    /// </summary>
    public string? Ytp002 { get; set; }
    /// <summary>
    /// YTP003 | Category Codes - Payroll3
    /// </summary>
    public string? Ytp003 { get; set; }
    /// <summary>
    /// YTP004 | Category Codes - Payroll4
    /// </summary>
    public string? Ytp004 { get; set; }
    /// <summary>
    /// YTUSER | User ID
    /// </summary>
    public string? Ytuser { get; set; }
    /// <summary>
    /// YTCMMT | Check Comment (Y/N)
    /// </summary>
    public char? Ytcmmt { get; set; }
    /// <summary>
    /// YTCKDT | Date - Pay Check
    /// </summary>
    public int? Ytckdt { get; set; }
    /// <summary>
    /// YTUAMT | Amount - Uprate
    /// </summary>
    public decimal? Ytuamt { get; set; }
    /// <summary>
    /// YTYST | Processed Code
    /// </summary>
    public char? Ytyst { get; set; }
    /// <summary>
    /// YTICU | Batch Number
    /// </summary>
    public int? Yticu { get; set; }
    /// <summary>
    /// YTGICU | General Ledger Batch Number
    /// </summary>
    public int? Ytgicu { get; set; }
    /// <summary>
    /// YTDICJ | Date - Batch (Julian)
    /// </summary>
    public int? Ytdicj { get; set; }
    /// <summary>
    /// YTUPMJ | Date - Updated
    /// </summary>
    public int? Ytupmj { get; set; }
    /// <summary>
    /// YTPID | Program ID
    /// </summary>
    public string? Ytpid { get; set; }
    /// <summary>
    /// YTANI | Account Number - Input (Mode Unknown)
    /// </summary>
    public string? Ytani { get; set; }
    /// <summary>
    /// YTCTRY | Century
    /// </summary>
    public int? Ytctry { get; set; }
    /// <summary>
    /// YTANN8 | Address Number-Provider/Trustee
    /// </summary>
    public int? Ytann8 { get; set; }
    /// <summary>
    /// YTPGRP | Deduct/Benefit Override Code
    /// </summary>
    public string? Ytpgrp { get; set; }
    /// <summary>
    /// YTPAYG | Effect on Gross Pay
    /// </summary>
    public char? Ytpayg { get; set; }
    /// <summary>
    /// YTPAYN | Effect on Net Pay
    /// </summary>
    public char? Ytpayn { get; set; }
    /// <summary>
    /// YTSFLG | Void Flag (Y/N)
    /// </summary>
    public char? Ytsflg { get; set; }
    /// <summary>
    /// YTWS | Work Schedule Code
    /// </summary>
    public char? Ytws { get; set; }
    /// <summary>
    /// YTPCK | Method of Printing
    /// </summary>
    public char? Ytpck { get; set; }
    /// <summary>
    /// YTICC | Interim Check Code
    /// </summary>
    public char? Yticc { get; set; }
    /// <summary>
    /// YTICS | Interim Check Status
    /// </summary>
    public char? Ytics { get; set; }
    /// <summary>
    /// YTCMTH | Shift Diff Calc Sequence
    /// </summary>
    public char? Ytcmth { get; set; }
    /// <summary>
    /// YTACO | Available DBA
    /// </summary>
    public char? Ytaco { get; set; }
    /// <summary>
    /// YTAI | Type - Sales
    /// </summary>
    public char? Ytai { get; set; }
    /// <summary>
    /// YTSEC$ | Security Indicator
    /// </summary>
    public char? Ytsec { get; set; }
    /// <summary>
    /// YTOHF | Overtime Code
    /// </summary>
    public char? Ytohf { get; set; }
    /// <summary>
    /// YTDEP1 | Deduction Period 1
    /// </summary>
    public char? Ytdep1 { get; set; }
    /// <summary>
    /// YTDEP2 | Deduction Period 2
    /// </summary>
    public char? Ytdep2 { get; set; }
    /// <summary>
    /// YTDEP3 | Deduction Period 3
    /// </summary>
    public char? Ytdep3 { get; set; }
    /// <summary>
    /// YTDEP4 | Deduction Period 4
    /// </summary>
    public char? Ytdep4 { get; set; }
    /// <summary>
    /// YTDEP5 | Deduction Period 5
    /// </summary>
    public char? Ytdep5 { get; set; }
    /// <summary>
    /// YTDEP6 | Deduction Period 6
    /// </summary>
    public char? Ytdep6 { get; set; }
    /// <summary>
    /// YTTT01 | Non-Taxable Authority Types 01
    /// </summary>
    public string? Yttt01 { get; set; }
    /// <summary>
    /// YTTT02 | Non-Taxable Authority Types 02
    /// </summary>
    public string? Yttt02 { get; set; }
    /// <summary>
    /// YTTT03 | Non-Taxable Authority Types 03
    /// </summary>
    public string? Yttt03 { get; set; }
    /// <summary>
    /// YTTT04 | Non-Taxable Authority Types 04
    /// </summary>
    public string? Yttt04 { get; set; }
    /// <summary>
    /// YTTT05 | Non-Taxable Authority Types 05
    /// </summary>
    public string? Yttt05 { get; set; }
    /// <summary>
    /// YTTT06 | Non-Taxable Authority Types 06
    /// </summary>
    public string? Yttt06 { get; set; }
    /// <summary>
    /// YTTT07 | Non-Taxable Authority Types 07
    /// </summary>
    public string? Yttt07 { get; set; }
    /// <summary>
    /// YTTT08 | Non-Taxable Authority Types 08
    /// </summary>
    public string? Yttt08 { get; set; }
    /// <summary>
    /// YTTT09 | Non-Taxable Authority Types 09
    /// </summary>
    public string? Yttt09 { get; set; }
    /// <summary>
    /// YTTT10 | Non-Taxable Authority Types 10
    /// </summary>
    public string? Yttt10 { get; set; }
    /// <summary>
    /// YTTT11 | Non-Taxable Authority Types 11
    /// </summary>
    public string? Yttt11 { get; set; }
    /// <summary>
    /// YTTT12 | Non-Taxable Authority Types 12
    /// </summary>
    public string? Yttt12 { get; set; }
    /// <summary>
    /// YTTT13 | Non-Taxable Authority Types 13
    /// </summary>
    public string? Yttt13 { get; set; }
    /// <summary>
    /// YTTT14 | Non-Taxable Authority Types 14
    /// </summary>
    public string? Yttt14 { get; set; }
    /// <summary>
    /// YTTT15 | Non-Taxable Authority Types 15
    /// </summary>
    public string? Yttt15 { get; set; }
    /// <summary>
    /// YTUSR | Payroll Lockout Identification
    /// </summary>
    public string? Ytusr { get; set; }
    /// <summary>
    /// YTEPA | Amount - Entered Gross Pay
    /// </summary>
    public decimal? Ytepa { get; set; }
    /// <summary>
    /// YTBFA | Benefit Factor
    /// </summary>
    public decimal? Ytbfa { get; set; }
    /// <summary>
    /// YTRTWC | Rate - Workers Comp Insurance
    /// </summary>
    public decimal? Ytrtwc { get; set; }
    /// <summary>
    /// YTGENR | Rate - Workers Comp Gen Liability
    /// </summary>
    public decimal? Ytgenr { get; set; }
    /// <summary>
    /// YTADV | Advance on Pay
    /// </summary>
    public char? Ytadv { get; set; }
    /// <summary>
    /// YTSTIP | Batch Timecard Offsite Flag
    /// </summary>
    public char? Ytstip { get; set; }
    /// <summary>
    /// YTALPH | Name - Alpha
    /// </summary>
    public string? Ytalph { get; set; }
    /// <summary>
    /// YTIIAP | Auto Pay Methods
    /// </summary>
    public char? Ytiiap { get; set; }
    /// <summary>
    /// YTFICM | Tax Calc Method
    /// </summary>
    public char? Ytficm { get; set; }
    /// <summary>
    /// YTDTAB | Table Code
    /// </summary>
    public string? Ytdtab { get; set; }
    /// <summary>
    /// YTDTSP | Date - Time Stamp
    /// </summary>
    public int? Ytdtsp { get; set; }
    /// <summary>
    /// YTYST1 | Status- Payroll 01
    /// </summary>
    public char? Ytyst1 { get; set; }
    /// <summary>
    /// YTCKCS | Spending Account Claim Number
    /// </summary>
    public string? Ytckcs { get; set; }
    /// <summary>
    /// YTALT0 | G/L Posting Code - Alternate 0
    /// </summary>
    public char? Ytalt0 { get; set; }
    /// <summary>
    /// YTPOS | Position ID
    /// </summary>
    public string? Ytpos { get; set; }
    /// <summary>
    /// YTACTB | Activity-Based Costing Activity Code
    /// </summary>
    public string? Ytactb { get; set; }
    /// <summary>
    /// YTABR1 | Managerial Analysis Code 1
    /// </summary>
    public string? Ytabr1 { get; set; }
    /// <summary>
    /// YTABT1 | Managerial Analysis Type 1
    /// </summary>
    public char? Ytabt1 { get; set; }
    /// <summary>
    /// YTABR2 | Managerial Analysis Code 2
    /// </summary>
    public string? Ytabr2 { get; set; }
    /// <summary>
    /// YTABT2 | Managerial Analysis Type 2
    /// </summary>
    public char? Ytabt2 { get; set; }
    /// <summary>
    /// YTABR3 | Managerial Analysis Code 3
    /// </summary>
    public string? Ytabr3 { get; set; }
    /// <summary>
    /// YTABT3 | Managerial Analysis Type 3
    /// </summary>
    public char? Ytabt3 { get; set; }
    /// <summary>
    /// YTABR4 | Managerial Analysis Code 4
    /// </summary>
    public string? Ytabr4 { get; set; }
    /// <summary>
    /// YTABT4 | Managerial Analysis Type 4
    /// </summary>
    public char? Ytabt4 { get; set; }
    /// <summary>
    /// YTBLGRT | Rate - Billing Rate
    /// </summary>
    public decimal? Ytblgrt { get; set; }
    /// <summary>
    /// YTRCHGAMT | Rate - Recharge Amount
    /// </summary>
    public decimal? Ytrchgamt { get; set; }
    /// <summary>
    /// YTFBLGRT | Rate - Billing Foreign
    /// </summary>
    public decimal? Ytfblgrt { get; set; }
    /// <summary>
    /// YTFRCHGAMT | Rate - Recharge Amount Foreign
    /// </summary>
    public decimal? Ytfrchgamt { get; set; }
    /// <summary>
    /// YTCRR | Currency Conversion Rate - Spot Rate
    /// </summary>
    public int? Ytcrr { get; set; }
    /// <summary>
    /// YTCRCD | Currency Code - From
    /// </summary>
    public string? Ytcrcd { get; set; }
    /// <summary>
    /// YTCRDC | Currency Code - To
    /// </summary>
    public string? Ytcrdc { get; set; }
    /// <summary>
    /// YTRCHGMODE | Recharge Mode
    /// </summary>
    public char? Ytrchgmode { get; set; }
    /// <summary>
    /// YTLTTP | Leave Type
    /// </summary>
    public string? Ytlttp { get; set; }
    /// <summary>
    /// YTRKID | Leave Request Number
    /// </summary>
    public int? Ytrkid { get; set; }
    /// <summary>
    /// YTTELKFLG | Flag - Time Entry Lockout
    /// </summary>
    public char? Yttelkflg { get; set; }
    /// <summary>
    /// YTAPPRCFLG | Timecard Offsite Flag
    /// </summary>
    public char? Ytapprcflg { get; set; }
    /// <summary>
    /// YTOTRULECD | Code - Overtime Rule
    /// </summary>
    public string? Ytotrulecd { get; set; }
    /// <summary>
    /// YTTSKID | Task Unique Key ID
    /// </summary>
    public int? Yttskid { get; set; }
    /// <summary>
    /// YTUPMT | Time - Last Updated
    /// </summary>
    public int? Ytupmt { get; set; }
    /// <summary>
    /// YTTAXX | Tax Identification Number
    /// </summary>
    public string? Yttaxx { get; set; }
    /// <summary>
    /// YTTELKPP | Public Flag
    /// </summary>
    public char? Yttelkpp { get; set; }
    /// <summary>
    /// YTSCTR | Sick Certificate Required
    /// </summary>
    public char? Ytsctr { get; set; }
    /// <summary>
    /// YTSCRX | Sick Certificate Received
    /// </summary>
    public char? Ytscrx { get; set; }
    /// <summary>
    /// YTSVH | Benefit/Accrual Type
    /// </summary>
    public char? Ytsvh { get; set; }
    /// <summary>
    /// YTPAYLIA | Pay In Advance
    /// </summary>
    public char? Ytpaylia { get; set; }
    /// <summary>
    /// YTTCFD | Timecard From Date
    /// </summary>
    public int? Yttcfd { get; set; }
    /// <summary>
    /// YTTCTD | Timecard Thru Date
    /// </summary>
    public int? Yttctd { get; set; }
    /// <summary>
    /// YTHWPD | Hours Worked Per Day
    /// </summary>
    public decimal? Ythwpd { get; set; }
    /// <summary>
    /// YTINSTID | Instance Identifier
    /// </summary>
    public string? Ytinstid { get; set; }
    /// <summary>
    /// YTTCUN | Union Timecard Override
    /// </summary>
    public char? Yttcun { get; set; }
    /// <summary>
    /// YTTCHC | Home Company Timecard Override
    /// </summary>
    public char? Yttchc { get; set; }
    /// <summary>
    /// YTTCHB | Home Business Unit Timecard Override
    /// </summary>
    public char? Yttchb { get; set; }
    /// <summary>
    /// YTTCJT | Job Type Timecard Override
    /// </summary>
    public char? Yttcjt { get; set; }
    /// <summary>
    /// YTTCJS | Job Step Timecard Override
    /// </summary>
    public char? Yttcjs { get; set; }
    /// <summary>
    /// YTTCRT | Hourly Rate Timecard Override
    /// </summary>
    public char? Yttcrt { get; set; }
    /// <summary>
    /// YTTCSC | Shift Code Timecard Override
    /// </summary>
    public char? Yttcsc { get; set; }
    /// <summary>
    /// YTTCSD | Shift Differential Timecard Override
    /// </summary>
    public char? Yttcsd { get; set; }
    /// <summary>
    /// YTTCBR | Billing Rate Timecard Override
    /// </summary>
    public char? Yttcbr { get; set; }
    /// <summary>
    /// YTTCPC | Piece Rate Timecard Override
    /// </summary>
    public char? Yttcpc { get; set; }
    /// <summary>
    /// YTTCWC | Workers Comp Code Timecard Override
    /// </summary>
    public char? Yttcwc { get; set; }
    /// <summary>
    /// YTTCWS | Workers Comp Sub Class Timecard Override
    /// </summary>
    public char? Yttcws { get; set; }
    /// <summary>
    /// YTTCJL | Business Unit Charge out Timecard Override
    /// </summary>
    public char? Yttcjl { get; set; }
    /// <summary>
    /// YTTCANI | Account Number Timecard Override
    /// </summary>
    public char? Yttcani { get; set; }
    /// <summary>
    /// YTTCRFLG | Timecard Reprocess Flag
    /// </summary>
    public char? Yttcrflg { get; set; }
    /// <summary>
    /// YTJBLC | Contract Labor Category
    /// </summary>
    public string? Ytjblc { get; set; }
    /// <summary>
    /// YTCOPX | Component Pay Flag
    /// </summary>
    public char? Ytcopx { get; set; }
    /// <summary>
    /// YTBPTX | Base Pay Transaction Number
    /// </summary>
    public int? Ytbptx { get; set; }
    /// <summary>
    /// YTCOPB | Component Pay Basis
    /// </summary>
    public decimal? Ytcopb { get; set; }
    /// <summary>
    /// YTCMED | Component Pay Calculation Method
    /// </summary>
    public char? Ytcmed { get; set; }
    /// <summary>
    /// YTCOPR | Component Pay Amount Or Rate
    /// </summary>
    public decimal? Ytcopr { get; set; }
    /// <summary>
    /// YTGLBN | General Liability Benefit Amount
    /// </summary>
    public decimal? Ytglbn { get; set; }
    /// <summary>
    /// YTWCEX | Workers' Comp Excess Amount
    /// </summary>
    public decimal? Ytwcex { get; set; }
    /// <summary>
    /// YTGLEX | General Liability Excess Amount
    /// </summary>
    public decimal? Ytglex { get; set; }
    /// <summary>
    /// YTLDID | Labor Distribution Period ID
    /// </summary>
    public string? Ytldid { get; set; }
    /// <summary>
    /// YTLDED | Labor Period Ending Date
    /// </summary>
    public int? Ytlded { get; set; }
    /// <summary>
    /// YTTTAP | Total Time Accounting Processing
    /// </summary>
    public char? Ytttap { get; set; }
    /// <summary>
    /// YTINVA | Invalid Labor Account Number
    /// </summary>
    public string? Ytinva { get; set; }
    /// <summary>
    /// YTINRA | Invalid Recharge Account Number
    /// </summary>
    public string? Ytinra { get; set; }
    /// <summary>
    /// YTINEA | Invalid Equipment Account Number
    /// </summary>
    public string? Ytinea { get; set; }
    /// <summary>
    /// YTCRFL | Timecard Corrections Flag
    /// </summary>
    public char? Ytcrfl { get; set; }
    /// <summary>
    /// YTCPTR | Correction Timecard Historical Link
    /// </summary>
    public int? Ytcptr { get; set; }
    /// <summary>
    /// YTPCOR | Processed Correction
    /// </summary>
    public char? Ytpcor { get; set; }
    /// <summary>
    /// YTAUSPTWW | State Payroll Tax State Where Worked
    /// </summary>
    public string? Ytausptww { get; set; }
    /// <summary>
    /// YTAUBP | Australia Payment Type
    /// </summary>
    public char? Ytaubp { get; set; }

}
