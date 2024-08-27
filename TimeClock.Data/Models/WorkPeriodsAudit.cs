namespace TimeClock.Data.Models;

public partial class WorkPeriodsAudit : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public string Field { get; set; } = null!;
    public string OldValue { get; set; } = null!;
    public string NewValue { get; set; } = null!;
    public DateTime DateTime { get; set; }
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
