using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;
public class JobTypeStepToEquipmentDto
{
    public Guid Id { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    public string? UnionCode { get; set; }
    public Guid EquipmentId { get; set; }
    public JobTypeDto? JobType { get; set; }
    public JobStepDto? JobStep { get; set; }
    public EquipmentDto? Equipment { get; set; }
}
