namespace TimeClock.Data.Models;

public partial class PunchEntry : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId {  get; set; }
    public Guid UserId { get; set; }
    public Guid WorkPeriodId { get; set; }
    public Guid DeviceId { get; set; }
    public virtual ICollection<PunchEntriesHistory> PunchEntriesHistories { get; set; } = new List<PunchEntriesHistory>();

    public virtual User User { get; set; } = null!;
    public virtual PunchEntriesCurrentState CurrentState { get; set; } = null!;
    public virtual WorkPeriod WorkPeriod { get; set; } = null!;
    public virtual Device Device { get; set; } = null!;
}
