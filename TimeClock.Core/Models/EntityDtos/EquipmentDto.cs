using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class EquipmentDto
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid EquipmentTypeId { get; set; }
    public EquipmentTypeDto? EquipmentType { get; set; }
    public IList<EquipmentsToUserDto> EquipmentsToUsers { get; set; } = [];
    public IList<EquipmentsToDepartmentLocationDto> EquipmentsToLocations { get; set; } = [];
    public IList<JobTypeStepToEquipmentDto> JobTypeStepToEquipment { get; set; } = [];
}