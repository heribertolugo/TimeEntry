namespace TimeClock.Data.Models;

public class PunchEntriesCurrentState : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId {  get; set; }
    public PunchStatus Status { get; set; }
    public PunchStatus? StableStatus { get; set; }
    public Guid PunchEntryId { get; set; }
    public Guid PunchEntriesHistoryId { get; set; }
    public Guid? StablePunchEntriesHistoryId { get; set; }
    public virtual PunchEntry PunchEntry { get; set; } = null!;
    public virtual PunchEntriesHistory PunchEntriesHistory { get; set; } = null!;
    public virtual PunchEntriesHistory? StablePunchEntriesHistory { get; set; } = null!;
}
