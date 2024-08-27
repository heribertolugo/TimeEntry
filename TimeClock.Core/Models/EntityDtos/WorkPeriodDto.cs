using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.EntityDtos;

public class WorkPeriodDto
{
    public Guid Id { get; set; }
    public double HoursWorked { get; set; }
    public DateTime WorkDate { get; set; }
    public Guid UserId { get; set; }
    public WorkPeriodPurposeDto Purpose { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
    public bool? IsPreviousMissingPunch { get; set; }
    public DateOnly? PreviousMissingPunchDate { get; set; }
    public UserDto? User { get; set; }
    public IList<WorkPeriodStatusHistoryDto> WorkPeriodStatusHistories { get; set; } = [];
    public IList<PunchEntryDto> PunchEntries { get; set; } = [];
    public IList<EquipmentsToUserDto> EquipmentsToUsers { get; set; } = [];
    public IList<WorkPeriodJobTypeStepDto> WorkPeriodJobTypeSteps { get; set; } = [];
}
