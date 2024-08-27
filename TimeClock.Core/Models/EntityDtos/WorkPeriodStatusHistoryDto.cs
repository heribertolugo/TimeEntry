using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.EntityDtos;

public class WorkPeriodStatusHistoryDto
{
    public Guid Id { get; set; }
    public Guid WorkPeriodId { get; set; }
    public WorkPeriodStatusDto Status { get; set; }
    public DateTime DateTime { get; set; }
    public WorkPeriodDto? WorkPeriod { get; set; }
}
