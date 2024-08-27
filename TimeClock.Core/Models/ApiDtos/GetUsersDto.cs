using System.Runtime.CompilerServices;
using TimeClock.Core.Models.EntityDtos;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;
internal class GetUsersDto : CanJson<GetUsersDto>
{
    public Guid DeviceId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? LocationId { get; set; }
    public bool? IsActive { get; set; }
    public IPagingDto Paging { get; set; } = new PagingDto(1, 1000);
    /// <summary>
    /// Specifies whether the users retrieved should be filtered to active or non-active users. 
    /// Use null to represent both states (all).
    /// </summary>
    public bool? UserActiveState { get; set; }
    public bool IncludeEquipmentToUser { get; set; }
    public bool IncludeClaims { get; set; }
    public bool IncludeJobType { get; set; }
    public bool IncludeJobStep { get; set; }
    public bool IncludeBarCode { get; set; }
    public bool IncludeJobTypeSteps { get; set; }
    public bool? IncludedJobTypeStepsActiveState{ get; set; }
}

internal class GetUserDto : CanJson<GetUserDto>
{
    public Guid DeviceId { get; set; }
    public Guid RequestedById { get; set; }
    public DateRange? PunchesDateRange { get; set; }
    public DateRange? WorkPeriodsDateRange { get; set; }
    public DateRange? EquipmentDateRange { get; set; }
    public Guid? UserId { get; set; }
    public int? JdeId { get; set; }
    /// <summary>
    /// States used to filter punches when PunchesDateRange is provided. 
    /// By default includes PunchAction(s) which represent a final approved and stable state, 
    /// excluding Void, None and any representing a Request.
    /// </summary>
    public HashSet<PunchActionDto> PunchesStates { get; set; } = new(PunchActionDtoEx.PositiveStates());
    /// <summary>
    /// Specifies whether the users retrieved should be filtered to active or non-active users. 
    /// Use null to represent both states (all).
    /// </summary>
    public bool? UserActiveState { get; set; }
    public bool IncludeClaims { get; set; }
    public bool IncludeJobType { get; set; }
    public bool IncludeJobStep { get; set; }
    public bool IncludeJobTypeSteps { get; set; }
}