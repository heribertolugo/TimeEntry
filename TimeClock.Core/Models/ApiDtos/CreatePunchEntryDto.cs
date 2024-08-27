using System.Runtime.CompilerServices;
using TimeClock.Core.Models.EntityDtos;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;

internal sealed class CreatePunchEntryDto : CanJson<CreatePunchEntryDto>
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid DeviceId { get; set; }
    public string? Password { get; set; }
    public DateTime DateTime { get; set; }
    //public DateTime EffectiveDateTime { get; set; }
    public Guid? ActionById { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public PunchActionDto PunchAction { get; set; }
    public PunchTypeDto PunchType { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    /// <summary>
    /// this flag is used to signify that the JobType and JobStep have been set after a redirect 
    /// in which a JobType and a JobStep were selected from multiple choice.
    /// </summary>
    public bool IsJobTypeStepSet { get; set; } = false;
    public Guid? WorkPeriodId { get; set;}
    public string? Note { get; set; }
    public bool IncludeUser { get; set; }
    public string? LocationDivisionCode { get; set; }
    public string? UnionCode { get; set; }
}
