using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class PunchEntriesHistoryDto
{
    public Guid Id { get; set; }
    public Guid PunchEntryId { get; set; }
    public Guid DeviceId { get; set; }
    public PunchTypeDto PunchType { get; set; }
    public DateTime? DateTime { get; set; }
    public DateTime? EffectiveDateTime { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public PunchActionDto Action { get; set; }
    public Guid ActionById { get; set; }
    public DateTime? UtcTimeStamp { get; set; }
    public string? Note { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    public UserDto? ActionBy { get; set; }
    public DeviceDto? Device { get; set; }
    public PunchEntryDto? PunchEntry { get; set; }
    public JobTypeDto? JobType { get; set; }
    public JobStepDto? JobStep { get; set; }
}
