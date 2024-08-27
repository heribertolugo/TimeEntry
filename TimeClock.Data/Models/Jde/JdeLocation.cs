namespace TimeClock.Data.Models.Jde;

/// <summary>
/// F0006 | Business Unit Master
/// </summary>
public partial class JdeLocation : IJdeEntityModel
{
    /// <summary>
    /// MCMCU | Business Unit
    /// </summary>
    /// <remarks>BusinessUnit</remarks>
    public string? BusinessUnit { get; set; } = " ";
    /// <summary>
    /// MCSTYL | Business Unit Type
    /// </summary>
    /// <remarks>
    /// BusinessUnitType
    /// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = 'MC' and MCSTYL = JdeCustomCode.Code
    /// </remarks>
    public string? BusinessUnitType { get; set; } = " ";
    ///// <summary>
    ///// MCDC | Description - Compressed
    ///// </summary>
    ///// <remarks>DescriptionCompressed</remarks>
    //public string? Mcdc { get; set; }
    ///// <summary>
    ///// MCLDM | Level of Detail - Business Unit
    ///// </summary>
    ///// <remarks>
    ///// LevelOfDetailBusinessUnit
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'LD' and MCLDM = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcldm { get; set; }
    /// <summary>
    /// MCCO | Company
    /// </summary>
    public string? Company { get; set; } = " ";
    /// <summary>
    /// MCAN8 | Address Number
    /// </summary>
    public int? AddressNumber { get; set; }
    ///// <summary>
    ///// MCAN8O | Address Number - Job A/R
    ///// </summary>
    ///// <remarks>AddressNumberJobAr</remarks>
    //public int? Mcan8o { get; set; }
    ///// <summary>
    ///// MCCNTY | County
    ///// </summary>
    ///// <remarks>
    ///// County
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = 'CT' and MCCNTY = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mccnty { get; set; }
    ///// <summary>
    ///// MCADDS | State
    ///// </summary>
    ///// <remarks>
    ///// State
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = 'S' and MCADDS = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcadds { get; set; }
    ///// <summary>
    ///// MCFMOD | Model Accounts and Consolidation Flag
    ///// </summary>
    ///// <remarks>
    ///// ModelAccountsAndConsolidationFlag
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H09' and JdeCustomCode.Codes = 'MD' and MCFMOD = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcfmod { get; set; }
    /// <summary>
    /// MCDL01 | Description
    /// </summary>
    public string? Description { get; set; } = " ";
    ///// <summary>
    ///// MCDL02 | Description 02
    ///// </summary>
    ///// <remarks>Description02</remarks>
    //public string? Mcdl02 { get; set; }
    ///// <summary>
    ///// MCDL03 | Description 03
    ///// </summary>
    ///// <remarks>Description03</remarks>
    //public string? Mcdl03 { get; set; }
    ///// <summary>
    ///// MCDL04 | Description 04
    ///// </summary>
    ///// <remarks>Description04</remarks>
    //public string? Mcdl04 { get; set; }
    /// <summary>
    /// MCRP01 | Division
    /// </summary>
    /// <remarks>
    /// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '01' and MCRP01 = JdeCustomCode.Code
    /// </remarks>
    public string? Division { get; set; }
    ///// <summary>
    ///// MCRP02 | Region
    ///// </summary>
    ///// <remarks>
    ///// Region
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '02' and MCRP02 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp02 { get; set; }
    ///// <summary>
    ///// MCRP03 | Group
    ///// </summary>
    ///// <remarks>
    ///// Group
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '03' and MCRP03 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp03 { get; set; }
    ///// <summary>
    ///// MCRP04 | Branch Office
    ///// </summary>
    ///// <remarks>
    ///// BranchOffice
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '04' and MCRP04 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp04 { get; set; }
    ///// <summary>
    ///// MCRP05 | Department Type
    ///// </summary>
    ///// <remarks>
    ///// DepartmentType
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '05' and MCRP05 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp05 { get; set; }
    ///// <summary>
    ///// MCRP06 | Person Responsible
    ///// </summary>
    ///// <remarks>
    ///// PersonResponsible
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '06' and MCRP06 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp06 { get; set; }
    ///// <summary>
    ///// MCRP07 | Line of Business
    ///// </summary>
    ///// <remarks>
    ///// LineOfBusiness
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '07' and MCRP07 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp07 { get; set; }
    ///// <summary>
    ///// MCRP08 | Category Code - Business Unit 08
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit08
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '08' and MCRP08 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp08 { get; set; }
    ///// <summary>
    ///// MCRP09 | Category Code - Business Unit 09
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit09
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '09' and MCRP09 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp09 { get; set; }
    ///// <summary>
    ///// MCRP10 | Category Code - Business Unit 10
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit10
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '10' and MCRP10 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp10 { get; set; }
    ///// <summary>
    ///// MCRP11 | Category Code - Business Unit 11
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit11
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '11' and MCRP11 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp11 { get; set; }
    ///// <summary>
    ///// MCRP12 | Category Code - Business Unit 12
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit12
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '12' and MCRP12 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp12 { get; set; }
    ///// <summary>
    ///// MCRP13 | Category Code - Business Unit 13
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit13
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '13' and MCRP13 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp13 { get; set; }
    ///// <summary>
    ///// MCRP14 | Category Code - Business Unit 14
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit14
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '14' and MCRP14 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp14 { get; set; }
    ///// <summary>
    ///// MCRP15 | Category Code - Business Unit 15
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit15
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '15' and MCRP15 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp15 { get; set; }
    ///// <summary>
    ///// MCRP16 | Category Code - Business Unit 16
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit16
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '16' and MCRP16 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp16 { get; set; }
    /// <summary>
    /// MCRP17 | Category Code - Business Unit 17
    /// </summary>
    /// <remarks>
    /// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '17' and MCRP17 = JdeCustomCode.Code
    /// </remarks>
    public string? CategoryCodeBusinessUnit17 { get; set; } = " ";
    ///// <summary>
    ///// MCRP18 | Category Code - Business Unit 18
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit18
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '18' and MCRP18 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp18 { get; set; }
    ///// <summary>
    ///// MCRP19 | Category Code - Business Unit 19
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit19
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '19' and MCRP19 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp19 { get; set; }
    ///// <summary>
    ///// MCRP20 | Category Code - Business Unit 20
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit20
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '20' and MCRP20 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp20 { get; set; }
    ///// <summary>
    ///// MCRP21 | Category Code - Business Unit 21
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit21
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '21' and MCRP21 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp21 { get; set; }
    ///// <summary>
    ///// MCRP22 | Category Code - Business Unit 22
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit22
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '22' and MCRP22 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp22 { get; set; }
    ///// <summary>
    ///// MCRP23 | Category Code - Business Unit 23
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit23
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '23' and MCRP23 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp23 { get; set; }
    ///// <summary>
    ///// MCRP24 | Category Code - Business Unit 24
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit24
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '24' and MCRP24 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp24 { get; set; }
    ///// <summary>
    ///// MCRP25 | Category Code - Business Unit 25
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit25
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '25' and MCRP25 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp25 { get; set; }
    ///// <summary>
    ///// MCRP26 | Category Code - Business Unit 26
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit26
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '26' and MCRP26 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp26 { get; set; }
    ///// <summary>
    ///// MCRP27 | Category Code - Business Unit 27
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit27
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '27' and MCRP27 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp27 { get; set; }
    ///// <summary>
    ///// MCRP28 | Category Code - Business Unit 28
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit28
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '28' and MCRP28 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp28 { get; set; }
    ///// <summary>
    ///// MCRP29 | Category Code - Business Unit 29
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit29
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '29' and MCRP29 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp29 { get; set; }
    ///// <summary>
    ///// MCRP30 | Category Code - Business Unit 30
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit30
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '30' and MCRP30 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp30 { get; set; }
    ///// <summary>
    ///// MCTA | Tax Area
    ///// </summary>
    ///// <remarks>TaxArea</remarks>
    //public string? Mcta { get; set; }
    ///// <summary>
    ///// MCTXJS | Tax Entity
    ///// </summary>
    ///// <remarks>TaxEntity</remarks>
    //public int? Mctxjs { get; set; }
    ///// <summary>
    ///// MCTXA1 | Tax Rate/Area
    ///// </summary>
    ///// <remarks>TaxRateArea</remarks>
    //public string? Mctxa1 { get; set; }
    ///// <summary>
    ///// MCEXR1 | Tax Expl Code 1
    ///// </summary>
    ///// <remarks>
    ///// TaxExplCode1
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = 'EX' and MCEXR1 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcexr1 { get; set; }
    ///// <summary>
    ///// MCTC01 | Tax/Deduction Codes 01
    ///// </summary>
    ///// <remarks>TaxDeductionCodes01</remarks>
    //public string? Mctc01 { get; set; }
    ///// <summary>
    ///// MCTC02 | Tax/Deduction Codes 02
    ///// </summary>
    ///// <remarks>TaxDeductionCodes02</remarks>
    //public string? Mctc02 { get; set; }
    ///// <summary>
    ///// MCTC03 | Tax/Deduction Codes 03
    ///// </summary>
    ///// <remarks>TaxDeductionCodes03</remarks>
    //public string? Mctc03 { get; set; }
    ///// <summary>
    ///// MCTC04 | Tax/Deduction Codes 04
    ///// </summary>
    ///// <remarks>TaxDeductionCodes04</remarks>
    //public string? Mctc04 { get; set; }
    ///// <summary>
    ///// MCTC05 | Tax/Deduction Codes 05
    ///// </summary>
    ///// <remarks>TaxDeductionCodes05</remarks>
    //public string? Mctc05 { get; set; }
    ///// <summary>
    ///// MCTC06 | Tax/Deduction Codes 06
    ///// </summary>
    ///// <remarks>TaxDeductionCodes06</remarks>
    //public string? Mctc06 { get; set; }
    ///// <summary>
    ///// MCTC07 | Tax/Deduction Codes 07
    ///// </summary>
    ///// <remarks>TaxDeductionCodes07</remarks>
    //public string? Mctc07 { get; set; }
    ///// <summary>
    ///// MCTC08 | Tax/Deduction Codes 08
    ///// </summary>
    ///// <remarks>TaxDeductionCodes08</remarks>
    //public string? Mctc08 { get; set; }
    ///// <summary>
    ///// MCTC09 | Tax/Deduction Codes 09
    ///// </summary>
    ///// <remarks>TaxDeductionCodes09</remarks>
    //public string? Mctc09 { get; set; }
    ///// <summary>
    ///// MCTC10 | Tax/Deduction Codes 10
    ///// </summary>
    ///// <remarks>TaxDeductionCodes10</remarks>
    //public string? Mctc10 { get; set; }
    ///// <summary>
    ///// MCND01 | Tax Distributable/Non-Distr. Code 01
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode01
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND01 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd01 { get; set; }
    ///// <summary>
    ///// MCND02 | Tax Distributable/Non-Distr. Code 02
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode02
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND02 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd02 { get; set; }
    ///// <summary>
    ///// MCND03 | Tax Distributable/Non-Distr. Code 03
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode03
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND03 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd03 { get; set; }
    ///// <summary>
    ///// MCND04 | Tax Distributable/Non-Distr. Code 04
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode04
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND04 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd04 { get; set; }
    ///// <summary>
    ///// MCND05 | Tax Distributable/Non-Distr. Code 05
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode05
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND05 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd05 { get; set; }
    ///// <summary>
    ///// MCND06 | Tax Distributable/Non-Distr. Code 06
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode06
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND06 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd06 { get; set; }
    ///// <summary>
    ///// MCND07 | Tax Distributable/Non-Distr. Code 07
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode07
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND07 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd07 { get; set; }
    ///// <summary>
    ///// MCND08 | Tax Distributable/Non-Distr. Code 08
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode08
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND08 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd08 { get; set; }
    ///// <summary>
    ///// MCND09 | Tax Distributable/Non-Distr. Code 09
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode09
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND09 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd09 { get; set; }
    ///// <summary>
    ///// MCND10 | Tax Distributable/Non-Distr. Code 10
    ///// </summary>
    ///// <remarks>
    ///// TaxDistributableNonDistrCode10
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'ND' and MCND10 = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcnd10 { get; set; }
    ///// <summary>
    ///// MCCC01 | Tax or Deduction Compute Status Code 01
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode01</remarks>
    //public char? Mccc01 { get; set; }
    ///// <summary>
    ///// MCCC02 | Tax or Deduction Compute Status Code 02
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode02</remarks>
    //public char? Mccc02 { get; set; }
    ///// <summary>
    ///// MCCC03 | Tax or Deduction Compute Status Code 03
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode03</remarks>
    //public char? Mccc03 { get; set; }
    ///// <summary>
    ///// MCCC04 | Tax or Deduction Compute Status Code 04
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode04</remarks>
    //public char? Mccc04 { get; set; }
    ///// <summary>
    ///// MCCC05 | Tax or Deduction Compute Status Code 05
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode05</remarks>
    //public char? Mccc05 { get; set; }
    ///// <summary>
    ///// MCCC06 | Tax or Deduction Compute Status Code 06
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode06</remarks>
    //public char? Mccc06 { get; set; }
    ///// <summary>
    ///// MCCC07 | Tax or Deduction Compute Status Code 07
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode07</remarks>
    //public char? Mccc07 { get; set; }
    ///// <summary>
    ///// MCCC08 | Tax or Deduction Compute Status Code 08
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode08</remarks>
    //public char? Mccc08 { get; set; }
    ///// <summary>
    ///// MCCC09 | Tax or Deduction Compute Status Code 09
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode09</remarks>
    //public char? Mccc09 { get; set; }
    ///// <summary>
    ///// MCCC10 | Tax or Deduction Compute Status Code 10
    ///// </summary>
    ///// <remarks>TaxOrDeductionComputeStatusCode10</remarks>
    //public char? Mccc10 { get; set; }
    ///// <summary>
    ///// MCPECC | Posting Edit - Business Unit
    ///// </summary>
    ///// <remarks>
    ///// PostingEditBusinessUnit
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = 'PF' and MCPECC = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcpecc { get; set; }
    ///// <summary>
    ///// MCALS | Allocation Summarization Method
    ///// </summary>
    ///// <remarks>
    ///// AllocationSummarizationMethod
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'AL' and MCALS = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcals { get; set; }
    ///// <summary>
    ///// MCISS | Invoice/Stmt Summarization Method
    ///// </summary>
    ///// <remarks>
    ///// InvoiceStmtSummarizationMethod
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'IS' and MCISS = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mciss { get; set; }
    ///// <summary>
    ///// MCGLBA | G/L Bank Account
    ///// </summary>
    ///// <remarks>GlbankAccount</remarks>
    //public string? Mcglba { get; set; }
    ///// <summary>
    ///// MCALCL | Allocation Level
    ///// </summary>
    ///// <remarks>AllocationLevel</remarks>
    //public string? Mcalcl { get; set; }
    ///// <summary>
    ///// MCLMTH | Labor Distribution Method
    ///// </summary>
    ///// <remarks>
    ///// LaborDistributionMethod
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '07' and JdeCustomCode.Codes = 'LT' and MCLMTH = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mclmth { get; set; }
    ///// <summary>
    ///// MCLF | Labor Distribution Multiplier
    ///// </summary>
    ///// <remarks>LaborDistributionMultiplier</remarks>
    //public decimal? Mclf { get; set; }
    ///// <summary>
    ///// MCOBJ1 | Object Account - Labor Account
    ///// </summary>
    ///// <remarks>ObjectAccountLaborAccount</remarks>
    //public string? Mcobj1 { get; set; }
    ///// <summary>
    ///// MCOBJ2 | Object Account - Premium Account
    ///// </summary>
    ///// <remarks>ObjectAccountPremiumAccount</remarks>
    //public string? Mcobj2 { get; set; }
    ///// <summary>
    ///// MCOBJ3 | Object Account - Burden
    ///// </summary>
    ///// <remarks>ObjectAccountBurden</remarks>
    //public string? Mcobj3 { get; set; }
    ///// <summary>
    ///// MCSUB1 | Subsidiary - Burden Cost Code
    ///// </summary>
    ///// <remarks>SubsidiaryBurdenCostCode</remarks>
    //public string? Mcsub1 { get; set; }
    ///// <summary>
    ///// MCTOU | Units - Total
    ///// </summary>
    ///// <remarks>UnitsTotal</remarks>
    //public decimal? Mctou { get; set; }
    ///// <summary>
    ///// MCSBLI | Subledger Inactive Code
    ///// </summary>
    ///// <remarks>
    ///// SubledgerInactiveCode
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = 'SI' and MCSBLI = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mcsbli { get; set; }
    ///// <summary>
    ///// MCANPA | Supervisor
    ///// </summary>
    ///// <remarks>Supervisor</remarks>
    //public int? Mcanpa { get; set; }
    ///// <summary>
    ///// MCCT | Contract Type
    ///// </summary>
    ///// <remarks>
    ///// ContractType
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '51' and JdeCustomCode.Codes = 'CT' and MCCT = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcct { get; set; }
    ///// <summary>
    ///// MCCERT | Certified Job
    ///// </summary>
    ///// <remarks>
    ///// CertifiedJob
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'CE' and MCCERT = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mccert { get; set; }
    ///// <summary>
    ///// MCMCUS | Business Unit - Subsequent
    ///// </summary>
    ///// <remarks>BusinessUnitSubsequent</remarks>
    //public string? Mcmcus { get; set; }
    ///// <summary>
    ///// MCBTYP | Billing Type - Business Unit
    ///// </summary>
    ///// <remarks>BillingTypeBusinessUnit</remarks>
    //public char? Mcbtyp { get; set; }
    ///// <summary>
    ///// MCPC | Percent Complete
    ///// </summary>
    ///// <remarks>PercentComplete</remarks>
    //public int? Mcpc { get; set; }
    ///// <summary>
    ///// MCPCA | Percent Complete - Aggregate Detail
    ///// </summary>
    ///// <remarks>PercentCompleteAggregateDetail</remarks>
    //public decimal? Mcpca { get; set; }
    ///// <summary>
    ///// MCPCC | Amount - Cost to Complete (obsolete)
    ///// </summary>
    ///// <remarks>AmountCostToCompleteObsolete</remarks>
    //public decimal? Mcpcc { get; set; }
    ///// <summary>
    ///// MCINTA | Interest Computation Code - A/R
    ///// </summary>
    ///// <remarks>InterestComputationCodeAr</remarks>
    //public string? Mcinta { get; set; }
    ///// <summary>
    ///// MCINTL | Interest Comp - Late Revenue
    ///// </summary>
    ///// <remarks>InterestCompLateRevenue</remarks>
    //public string? Mcintl { get; set; }
    ///// <summary>
    ///// MCD1J | Date - Planned Start
    ///// </summary>
    ///// <remarks>DatePlannedStart</remarks>
    //public int? Mcd1j { get; set; }
    ///// <summary>
    ///// MCD2J | Date - Actual Start
    ///// </summary>
    ///// <remarks>DateActualStart</remarks>
    //public int? Mcd2j { get; set; }
    ///// <summary>
    ///// MCD3J | Date - Planned Complete
    ///// </summary>
    ///// <remarks>DatePlannedComplete</remarks>
    //public int? Mcd3j { get; set; }
    ///// <summary>
    ///// MCD4J | Date - Actual Complete
    ///// </summary>
    ///// <remarks>DateActualComplete</remarks>
    //public int? Mcd4j { get; set; }
    ///// <summary>
    ///// MCD5J | Date - Other 5
    ///// </summary>
    ///// <remarks>DateOther5</remarks>
    //public int? Mcd5j { get; set; }
    ///// <summary>
    ///// MCD6J | Date - Other 6
    ///// </summary>
    ///// <remarks>DateOther6</remarks>
    //public int? Mcd6j { get; set; }
    ///// <summary>
    ///// MCFPDJ | Date - Final Payment (Julian)
    ///// </summary>
    ///// <remarks>DateFinalPaymentJulian</remarks>
    //public int? Mcfpdj { get; set; }
    ///// <summary>
    ///// MCCAC | Amount - Cost at Completion
    ///// </summary>
    ///// <remarks>AmountCostAtCompletion</remarks>
    //public decimal? Mccac { get; set; }
    ///// <summary>
    ///// MCPAC | Amount - Profit at Completion
    ///// </summary>
    ///// <remarks>AmountProfitAtCompletion</remarks>
    //public decimal? Mcpac { get; set; }
    ///// <summary>
    ///// MCEEO | Equal Employment Opportunity
    ///// </summary>
    ///// <remarks>
    ///// EqualEmploymentOpportunity
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = 'H00' and JdeCustomCode.Codes = 'EE' and MCEEO = JdeCustomCode.Code
    ///// </remarks>
    //public char? Mceeo { get; set; }
    ///// <summary>
    ///// MCERC | Equipment Rate Code
    ///// </summary>
    ///// <remarks>
    ///// EquipmentRateCode
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = 'RC' and MCERC = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcerc { get; set; }
    ///// <summary>
    ///// MCUSER | User ID
    ///// </summary>
    ///// <remarks>UserId</remarks>
    //public string? Mcuser { get; set; }
    ///// <summary>
    ///// MCPID | Program ID
    ///// </summary>
    ///// <remarks>ProgramId</remarks>
    //public string? Mcpid { get; set; }
    ///// <summary>
    ///// MCUPMJ | Date - Updated
    ///// </summary>
    ///// <remarks>DateUpdated</remarks>
    //public int? Mcupmj { get; set; }
    ///// <summary>
    ///// MCJOBN | Work Station ID
    ///// </summary>
    ///// <remarks>WorkStationId</remarks>
    //public string? Mcjobn { get; set; }
    ///// <summary>
    ///// MCUPMT | Time - Last Updated
    ///// </summary>
    ///// <remarks>TimeLastUpdated</remarks>
    //public int? Mcupmt { get; set; }
    ///// <summary>
    ///// MCBPTP | Branch Type
    ///// </summary>
    ///// <remarks>BranchType</remarks>
    //public string? Mcbptp { get; set; }
    ///// <summary>
    ///// MCAPSB | APS Business Unit
    ///// </summary>
    ///// <remarks>ApsBusinessUnit</remarks>
    //public char? Mcapsb { get; set; }
    ///// <summary>
    ///// MCTSBU | Target Business Unit
    ///// </summary>
    ///// <remarks>TargetBusinessUnit</remarks>
    //public string? Mctsbu { get; set; }
    ///// <summary>
    ///// MCRP31 | Category Code - Business Unit 31
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit31
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '31' and MCRP31 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp31 { get; set; }
    ///// <summary>
    ///// MCRP32 | Category Code - Business Unit 32
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit32
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '32' and MCRP32 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp32 { get; set; }
    ///// <summary>
    ///// MCRP33 | Category Code - Business Unit 33
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit33
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '33' and MCRP33 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp33 { get; set; }
    ///// <summary>
    ///// MCRP34 | Category Code - Business Unit 34
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit34
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '34' and MCRP34 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp34 { get; set; }
    ///// <summary>
    ///// MCRP35 | Category Code - Business Unit 35
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit35
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '35' and MCRP35 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp35 { get; set; }
    ///// <summary>
    ///// MCRP36 | Category Code - Business Unit 36
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit36
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '36' and MCRP36 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp36 { get; set; }
    ///// <summary>
    ///// MCRP37 | Category Code - Business Unit 37
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit37
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '37' and MCRP37 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp37 { get; set; }
    ///// <summary>
    ///// MCRP38 | Category Code - Business Unit 38
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit38
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '38' and MCRP38 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp38 { get; set; }
    ///// <summary>
    ///// MCRP39 | Category Code - Business Unit 39
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit39
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '39' and MCRP39 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp39 { get; set; }
    ///// <summary>
    ///// MCRP40 | Category Code - Business Unit 40
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit40
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '40' and MCRP40 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp40 { get; set; }
    ///// <summary>
    ///// MCRP41 | Category Code - Business Unit 41
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit41
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '41' and MCRP41 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp41 { get; set; }
    ///// <summary>
    ///// MCRP42 | Category Code - Business Unit 42
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit42
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '42' and MCRP42 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp42 { get; set; }
    ///// <summary>
    ///// MCRP43 | Category Code - Business Unit 43
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit43
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '43' and MCRP43 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp43 { get; set; }
    ///// <summary>
    ///// MCRP44 | Category Code - Business Unit 44
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit44
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '44' and MCRP44 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp44 { get; set; }
    ///// <summary>
    ///// MCRP45 | Category Code - Business Unit 45
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit45
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '45' and MCRP45 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp45 { get; set; }
    ///// <summary>
    ///// MCRP46 | Category Code - Business Unit 46
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit46
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '46' and MCRP46 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp46 { get; set; }
    ///// <summary>
    ///// MCRP47 | Category Code - Business Unit 47
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit47
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '47' and MCRP47 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp47 { get; set; }
    ///// <summary>
    ///// MCRP48 | Category Code - Business Unit 48
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit48
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '48' and MCRP48 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp48 { get; set; }
    ///// <summary>
    ///// MCRP49 | Category Code - Business Unit 49
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit49
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '49' and MCRP49 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp49 { get; set; }
    ///// <summary>
    ///// MCRP50 | Category Code - Business Unit 50
    ///// </summary>
    ///// <remarks>
    ///// CategoryCodeBusinessUnit50
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '00' and JdeCustomCode.Codes = '50' and MCRP50 = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcrp50 { get; set; }
    ///// <summary>
    ///// MCAN8GCA1 | Address Number1
    ///// </summary>
    ///// <remarks>AddressNumber1</remarks>
    //public int? Mcan8gca1 { get; set; }
    ///// <summary>
    ///// MCAN8GCA2 | Address Number2
    ///// </summary>
    ///// <remarks>AddressNumber2</remarks>
    //public int? Mcan8gca2 { get; set; }
    ///// <summary>
    ///// MCAN8GCA3 | Address Number3
    ///// </summary>
    ///// <remarks>AddressNumber3</remarks>
    //public int? Mcan8gca3 { get; set; }
    ///// <summary>
    ///// MCAN8GCA4 | Address Number4
    ///// </summary>
    ///// <remarks>AddressNumber4</remarks>
    //public int? Mcan8gca4 { get; set; }
    ///// <summary>
    ///// MCAN8GCA5 | Address Number5
    ///// </summary>
    ///// <remarks>AddressNumber5</remarks>
    //public int? Mcan8gca5 { get; set; }
    ///// <summary>
    ///// MCRMCU1 | Related Business Unit
    ///// </summary>
    ///// <remarks>RelatedBusinessUnit</remarks>
    //public string? Mcrmcu1 { get; set; }
    ///// <summary>
    ///// MCDOCO | Document (Order No Invoice etc.)
    ///// </summary>
    ///// <remarks>DocumentOrderNoInvoiceEtc</remarks>
    //public int? Mcdoco { get; set; }
    ///// <summary>
    ///// MCPCTN | Parent Contract Number
    ///// </summary>
    ///// <remarks>ParentContractNumber</remarks>
    //public int? Mcpctn { get; set; }
    ///// <summary>
    ///// MCCLNU | Contract Level Number
    ///// </summary>
    ///// <remarks>ContractLevelNumber</remarks>
    //public int? Mcclnu { get; set; }
    ///// <summary>
    ///// MCBUCA | Burden Category
    ///// </summary>
    ///// <remarks>
    ///// BurdenCategory
    ///// Join to JdeCustomCode on JdeCustomCode.ProductCode = '48S' and JdeCustomCode.Codes = 'BC' and MCBUCA = JdeCustomCode.Code
    ///// </remarks>
    //public string? Mcbuca { get; set; }
    ///// <summary>
    ///// MCADJENT | Adjustment Entry
    ///// </summary>
    ///// <remarks>AdjustmentEntry</remarks>
    //public char? Mcadjent { get; set; }
    ///// <summary>
    ///// MCUAFL | FAR UnAllowable Flag
    ///// </summary>
    ///// <remarks>FarUnallowableFlag</remarks>
    //public char? Mcuafl { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual ICollection<JdeEquipmentLocation> EquipmentLocations { get; set; } = [];
}