using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class EquipmentsToDepartmentLocationDto
{
    public Guid Id { get; set; }
    public DateTime LinkedOn { get; set; }
    public Guid EquipmentId { get; set; }
    public Guid DepartmentsToLocationId { get; set; }
    public Guid LinkedById { get; set; }
    public bool IsActive { get; set; }
    public EquipmentDto? Equipment { get; set; }
    public DepartmentsToLocationDto? DepartmentsToLocation { get; set; }
    public UserDto? LinkedBy { get; set; }
}