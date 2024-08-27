using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.EntityDtos;

public class WorkPeriodJobTypeStepDto
{
    public Guid Id { get; set; }
    public DateTime ActiveSince { get; set; }
    public Guid WorkPeriodId { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    public string? JobTypeDescription { get; set; }
    public bool? JobStepIsActive { get; set; }
    public string? JobStepDescription { get; set; }
    public bool? JobTypeIsActive { get; set; }
    public string? JobTypeJdeId { get; set; }
    public string? JobStepJdeId { get; set; }
}
