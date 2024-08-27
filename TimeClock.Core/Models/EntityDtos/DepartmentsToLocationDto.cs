using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class DepartmentsToLocationDto
{
    public Guid Id { get; set; }
    public Guid LocationId { get; set; }
    public Guid DepartmentId { get; set; }
    public LocationDto? Location { get; set; }
    public DepartmentDto? Department { get; set; }
    public IList<DeviceDto> Devices { get; set; } = [];
}
