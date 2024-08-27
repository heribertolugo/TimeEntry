using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

public partial class EquipmentsToDepartmentLocation : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public Guid DepartmentsToLocationId { get; set; }

    public Guid EquipmentId { get; set; }

    public DateTime? LinkedOn { get; set; }

    public Guid LinkedById { get; set; }

    public bool IsActive {  get; set; }

    public int? JdeId { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual User LinkedBy { get; set; } = null!;

    public virtual DepartmentsToLocation DepartmentsToLocation { get; set; } = null!;
    [NotMapped]
    object? IReferenceJde.JdeId { get => this.JdeId; set => this.JdeId = value as int?; }
}
