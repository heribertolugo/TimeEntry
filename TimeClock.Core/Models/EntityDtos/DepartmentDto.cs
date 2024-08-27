using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class DepartmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public IList<DepartmentsToLocationDto> DepartmentsToLocations { get; set; } = [];
}
