using System.Runtime.CompilerServices;
using TimeClock.Core.Models.EntityDtos;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;

internal class GetWorkPeriodDto : CanJson<GetWorkPeriodDto>
{
    public GetWorkPeriodDto()
    {

    }
    public Guid? Id { get; set; }
    public Guid DeviceId { get; set; }
    /// <summary>
    /// If specified, only work periods for the specified user will be retrieved.
    /// </summary>
    public Guid? UserId { get; set; }
    /// <summary>
    /// Specifies the User who is requesting the information.
    /// </summary>
    public Guid RequestedById { get; set; }
    public IList<Guid> JobTypeIds { get; set; } = [];
    public DateTime? WorkDate { get; set; }
    public bool? IsProcessed { get; set; } = false;
    public bool IncludeUser { get; set; }
    public bool IncludePunchEntries { get; set; }
    public bool IncludeEquipment { get; set; }
    //public bool IncludeJobType { get; set; }
    //public bool IncludeJobStep { get; set; }
    public HashSet<PunchActionDto> PunchActions { get; set; }
}


internal class GetWorkPeriodsDto : CanJson<GetWorkPeriodsDto>
{
    public GetWorkPeriodsDto()
    {
    }
    public Guid DeviceId { get; set; }
    /// <summary>
    /// If specified, only work periods for the specified user will be retrieved.
    /// </summary>
    public Guid? UserId { get; set; }
    /// <summary>
    /// Specifies the User who is requesting the information.
    /// </summary>
    public Guid RequestedById { get; set; }
    ///// <summary>
    ///// Restricts the results by the specified JobType(s)
    ///// </summary>
    //public IList<Guid> JobTypeIds { get; set; } = [];
    public DateRange? DateRange { get; set; }
    public bool? IsProcessed { get; set; } = false;
    public bool IncludeUser { get; set; }
    public bool IncludePunchEntries { get; set; }
    public bool IncludeEquipment { get; set; }
    //public bool IncludeJobType { get; set; }
    //public bool IncludeJobStep { get; set; }
    public IPagingDto? Paging { get; set; }
    public HashSet<PunchActionDto> PunchActions { get; set; }
}
