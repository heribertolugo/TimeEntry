using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class PunchEntryDto
{
    public PunchEntryDto() { }
    public PunchEntryDto(Guid id = default, Guid userId = default, Guid deviceId = default,
        PunchTypeDto punchType = default, DateTime? dateTime = null, DateTime? effectiveDateTime = null,
        double? latitude = default, double? longitude = default, PunchActionDto action = default,
        Guid actionById = default, DateTime? utcTimeStamp = null,
        UserDto? user = null, DeviceDto? device = null, UserDto? actionBy = null,
        IEnumerable<PunchEntriesHistoryDto>? punchEntriesHistories = null, Guid historyId = default, Guid currentStateId = default,
        Guid? workPeriodId = null, WorkPeriodDto? workPeriod = null, string? note = null,
         Guid? stableStateId = null, PunchEntriesHistoryDto? stableState = null)
    {
        this.Id = id;
        this.UserId = userId;
        this.DeviceId = deviceId;
        this.PunchType = punchType;
        this.DateTime = dateTime.HasValue ? dateTime.Value : default;
        this.EffectiveDateTime = effectiveDateTime.HasValue ? effectiveDateTime.Value : default;
        this.Latitude = latitude;
        this.Longitude = longitude;
        this.Action = action;
        this.ActionById = actionById;
        this.UtcTimeStamp = utcTimeStamp.HasValue ? utcTimeStamp.Value : default;
        this.User = user;
        this.Device = device;
        this.ActionBy = actionBy;
        this.PunchEntriesHistories = punchEntriesHistories is null ?
            new List<PunchEntriesHistoryDto>() : new List<PunchEntriesHistoryDto>(punchEntriesHistories);
        this.HistoryId = historyId;
        this.CurrentStateId = currentStateId;
        this.WorkPeriodId = workPeriodId.HasValue ? workPeriodId.Value : default;
        this.WorkPeriod = workPeriod;
        this.Note = note;
        this.StableStateId = stableStateId;
        this.StableState = stableState;
    }

    public Guid Id { get; set; }
    public Guid HistoryId { get; set; }
    public Guid CurrentStateId { get; set; }
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
    public Guid? StableStateId { get; set; }
    public PunchTypeDto PunchType { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime EffectiveDateTime { get; set; }
    public DateTime? StableDateTime { get; set; }
    public DateTime? StableEffectiveDateTime { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public PunchActionDto Action { get; set; }
    public PunchActionDto? StableAction { get; set; }
    public Guid ActionById { get; set; }
    public Guid WorkPeriodId { get; set; }
    public DateTime UtcTimeStamp { get; set; }
    public PunchStatusDto PunchStatus { get; set; }
    public PunchStatusDto? StablePunchStatus { get; set; }
    public string? Note { get; set; }
    public Guid? JobStepId { get; set; }
    public Guid? JobTypeId { get; set; }
    public UserDto? User { get; set; }
    public DeviceDto? Device { get; set; }
    public UserDto? ActionBy { get; set; }
    public WorkPeriodDto? WorkPeriod { get; set; }
    public PunchEntriesHistoryDto? StableState { get; set; }
    public JobStepDto? JobStep { get; set; }
    public JobTypeDto? JobType { get; set; }
    public IList<PunchEntriesHistoryDto> PunchEntriesHistories { get; set; } = new List<PunchEntriesHistoryDto>();
}

public enum PunchStatusDto
{
    In,
    Out
}
