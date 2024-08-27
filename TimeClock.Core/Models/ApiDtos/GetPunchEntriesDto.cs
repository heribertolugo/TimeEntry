using System.Runtime.CompilerServices;
using TimeClock.Core.Models.EntityDtos;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;

internal sealed class GetPunchEntriesDto : CanJson<GetPunchEntriesDto>
{
    public GetPunchEntriesDto() { }
    public Guid DeviceId { get; set; }
    public Guid RequestedById { get; set; }
    /// <summary>
    /// Provide UserId to filter by that user 
    /// </summary>
    public Guid? UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? LocationId { get; set; }
    public DateRange DateRange { get; set; }
    //public DateTime From { get; set; }
    //public DateTime To { get; set; }
    public IPagingDto? Paging { get; set; }

    /// <summary>
    /// By default includes PunchAction(s) which represent a final approved and stable state, 
    /// excluding Void, None and any representing a Request.
    /// </summary>
    public HashSet<PunchActionDto> CurrentStates { get; set; }
    public bool IncludeUser { get; set; }
    public bool IncludeHistory { get; set; }
    public bool IncludeDepartment { get; set; }
    public bool IncludeLocation { get; set; }
    public bool IncludeWorkPeriod { get; set; }
    public bool IncludeWorkPeriodJobType { get; set; }
    public bool IncludeStableState { get; set; }
    public bool GetIfStableState { get; set; }
    /// <summary>
    /// Includes <see cref="EquipmentDto"/> using the <see cref="DateRange.Start"/> and <see cref="DateRange.End"/> specified
    /// </summary>
    public bool IncludeEquipment { get; set; }
    public IList<SortOption<GetPunchEntriesDto.SortField>> Sorting { get; set; } = [];

    public enum SortField
    {
        DateTime,
        User,
    }
}

internal sealed class GetPunchEntryDto : CanJson<GetPunchEntryDto>
{
    public Guid Id { get; set; }
    public Guid DeviceId { get; set; }
    public Guid RequestedById { get; set; }
    public bool IncludeUser { get; set; }
    public bool IncludeHistory { get; set; }
}
