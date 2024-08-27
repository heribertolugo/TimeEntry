namespace TimeClock.Data.Models.Jde;

/// <summary>
/// F060116 | Employee Master Information
/// </summary>
public partial class JdeEmployee : IJdeEntityModel
{
    /// <summary>
    /// YAAN8 | Address Number
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// YAALPH | Name - Alpha
    /// </summary>
    public string Name { get; set; } = " ";
    /// <summary>
    /// YAHMCU | Business Unit - Home
    /// </summary>
    public string LocationId { get; set; } = " "; // Yasloc <- location | hmcu = locality
    /// <summary>
    /// YAJBCD | Job Type (Craft) Code
    /// </summary>
    /// <remarks>Join to JdeCustomCode on JdeCustomCode.ProductCode = '06' and JdeCustomCode.Codes = 'G' and JobType = JdeCustomCode.Code</remarks>
    public string JobType { get; set; } = " ";
    /// <summary>
    /// YAJBST | Job Step
    /// </summary>
    /// <remarks>Join to JdeCustomCode on JdeCustomCode.ProductCode = '06' and JdeCustomCode.Codes = 'GS' and JobStep = JdeCustomCode.Code</remarks>
    public string JobStep { get; set; } = " ";
    /// <summary>
    /// YAPAST | Employee Pay Status ('0') = active
    /// </summary>
    public char PayStatus { get; set; }
    /// <summary>
    /// YAOEMP | Additional Employee No
    /// </summary>
    public string EmployeeId { get; set; } = " ";
    /// <summary>
    /// YAHMCO | Company - Home
    /// </summary>
    public string Company { get; set; } = " ";
    /// <summary>
    /// YAUN | Union Code
    /// </summary>
    public string? UnionCode { get; set; }
    /// <summary>
    /// YAANPA | Supervisor AN8
    /// </summary>
    public int? SupervisorId { get; set; }

    public virtual JdeLocation Location { get; set; } = null!;
    public virtual JdeEmployee? Supervisor { get; set; }
    public virtual ICollection<JdeEmployee> Subordinates { get; set; } = [];
    public virtual ICollection<JdeEmail> Emails { get; set; } = [];
}
