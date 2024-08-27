using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

public partial class Department : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }

    public int RowId { get; set; }

    public string Name { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string? JdeId { get; set; }

    public virtual ICollection<DepartmentsToLocation> DepartmentsToLocations { get; set; } = new List<DepartmentsToLocation>();
    [NotMapped]
    object? IReferenceJde.JdeId { get => this.JdeId; set => this.JdeId = value as string; }
}
