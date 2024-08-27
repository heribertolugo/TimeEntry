namespace TimeClock.Data.Models;

public partial class PunchEntriesHistory : IEntityModel
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public Guid PunchEntryId { get; set; }

    public Guid DeviceId { get; set; }

    public PunchType PunchType { get; set; }

    public DateTime? DateTime { get; set; }

    public DateTime? EffectiveDateTime { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public PunchAction Action { get; set; }

    public Guid ActionById { get; set; }

    public DateTime? UtcTimeStamp { get; set; }

    public string? Note { get; set; }

    public Guid? JobTypeId { get; set; }

    public Guid? JobStepId { get; set; }

    public virtual User ActionBy { get; set; } = null!;

    public virtual Device Device { get; set; } = null!;

    public virtual PunchEntry PunchEntry { get; set; } = null!;

    public virtual PunchEntriesCurrentState? CurrentState { get; set; } = null!;

    public virtual JobType? JobType { get; set; }

    public virtual JobStep? JobStep { get; set; }
}
