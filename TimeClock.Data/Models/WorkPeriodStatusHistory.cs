namespace TimeClock.Data.Models;

/// <summary>
/// Keeps track of submission and processing status
/// </summary>
public partial class WorkPeriodStatusHistory : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public Guid WorkPeriodId { get; set; }
    public WorkPeriodStatus Status { get; set; }
    public DateTime DateTime { get; set; }
    public virtual WorkPeriod WorkPeriod { get; set; } = null!;
    public string? JdeId { get; set; } = null;
    object? IReferenceJde.JdeId{ get => this.JdeId; set => this.JdeId = value?.ToString(); }
}
