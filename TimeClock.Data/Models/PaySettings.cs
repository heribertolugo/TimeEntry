namespace TimeClock.Data.Models;

public class PaySettings : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public int StraightTimeDba { get; set; }
    public int OvertimeDba { get; set; }
    public string StraightTimeGlCode { get; set; } = null!;
    public string OvertimeGlCode { get; set; } = null!;
    public string CompanyId { get; set; } = null!;
    public Guid? DefaultOvertimeRuleId { get; set; }

    /// <summary>
    /// Required
    /// </summary>
    public virtual DefaultOvertimeRule DefaultOvertimeRule { get; set; } = null!;
    /// <summary>
    /// Optional
    /// </summary>
    public virtual ICollection<SpecialOvertimeRule> SpecialOvertimeRules { get; set; } = [];
    /// <summary>
    /// Optional
    /// </summary>
    public virtual ICollection<JobTypePayRule> JobTypePayRules { get; set; } = [];
}
