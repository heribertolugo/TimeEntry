using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

public partial class Equipment : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Guid EquipmentTypeId { get; set; }

    public bool? IsActive { get; set; }

    public int? JdeId { get; set; }

    public virtual EquipmentType EquipmentType { get; set; } = null!;

    public virtual ICollection<EquipmentsToDepartmentLocation> EquipmentsToLocations { get; set; } = [];

    public virtual ICollection<EquipmentsToUser> EquipmentsToUsers { get; set; } = [];

    public virtual ICollection<JobTypeStepToEquipment> JobTypeStepToEquipment { get; set; } = [];
    [NotMapped]
    object? IReferenceJde.JdeId { get => this.JdeId; set => this.JdeId = value as int?; }
}
