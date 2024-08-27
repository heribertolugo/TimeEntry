using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.EntityDtos;

public enum WorkPeriodStatusDto
{
    Pending,
    Submitted,
    Accepted,
    Rejected
}
