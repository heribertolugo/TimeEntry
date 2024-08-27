using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;
internal class GetEquipmentsDto : CanJson<GetEquipmentsDto>
{
    public Guid? UserId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? LocationId { get; set; }
    public bool? IsActive { get; set; }
    public IPagingDto Paging { get; set; } = new PagingDto(1,1000);

    public UnlinkedOption Unlinked { get; set; }
    /// <summary>
    /// This property is only relevant when <see cref="GetEquipmentsDto.Unlinked"/> is set to <see cref="GetEquipmentsDto.UnlinkedOption.DateRange" />
    /// </summary>
    public DateRange? UnlinkedDateRange { get; set; }
    public DateRange? LinkedDateRange { get; set; }
    /// <summary>
    /// This will include any unlinked EquipmentToUser relationship object. 
    /// If used with <see cref="IncludeEquipmentToUserUser"/>, 
    /// this flag will act as a filter to only include unlinked EquipmentToUser relationship objects. 
    /// Application logic should dictate that there should only ever be 1 unlinked max at any given time. 
    /// However, this is not enforced in Database.
    /// </summary>
    public bool IncludeCurrentEquipmentToUser { get; set; }
    /// <summary>
    /// Allows inclusion of EquipmentToUser relationship objects and the corresponding User object associated.
    /// </summary>
    public bool IncludeEquipmentToUserUser { get; set; }
    /// <summary>
    /// Allows for inclusion of EquipmentToUser based on a specific User in a specified WorkPeriod.
    /// </summary>
    public UserAndWorkPeriod? HistoryForUserAndWorkPeriod { get; set; }
    /// <summary>
    /// Allows inclusion of JobType - JobStep combinations mapped to equipment
    /// </summary>
    public bool IncludeEquipmentJobTypeSteps { get; set; }

    public IList<SortOption<GetEquipmentsDto.SortField>> Sorting { get; set; } = [];

    public enum UnlinkedOption
    {
        None,
        /// <summary>
        /// Value of UnlinkedOn is NULL
        /// </summary>
        Null,
        /// <summary>
        /// Value of UnlinkedOn is NOT NULL
        /// </summary>
        NotNull,
        /// <summary>
        /// Value of UnlinkedOn is within a date range
        /// </summary>
        DateRange
    }

    public enum SortField
    {
        User,
        DepartmentLocation,
        Sku,
        Name,
        EquipmentType,
        LastUsed
    }

    public struct UserAndWorkPeriod(Guid userId, DateTime workPeriodDate)
    {
        public Guid UserId { get; private set; } = userId;
        public DateTime WorkPeriodDate { get; private set; } = workPeriodDate;
    }
}
