namespace TimeClock.Data.Models;

public class DefaultOvertimeRule : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    /// <summary>
    /// Number of hours to threshold after which overtime will begin
    /// </summary>
    public decimal AfterHours { get; set; }
    /// <summary>
    /// On what occurrence the <see cref="AfterHours"/> threshold is reach (e.g: Day, Week)
    /// </summary>
    public OvertimeThresholdType OvertimeThreshold { get; set; }
    public Guid? SpecialPayId { get; set; }
    public Guid PaySettingsId { get; set; }

    public virtual SpecialPay? SpecialPay { get; set; }
    public virtual PaySettings PaySettings { get; set; } = null!;
}
