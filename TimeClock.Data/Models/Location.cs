using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

public partial class Location : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public string Name { get; set; } = null!;

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public bool? IsActive { get; set; }

    public string? JdeId { get; set; }

    public string? DivisionCode { get; set; }

    public virtual ICollection<DepartmentsToLocation> DepartmentsToLocations { get; set; } = new List<DepartmentsToLocation>();
    [NotMapped]
    object? IReferenceJde.JdeId { get => this.JdeId; set => this.JdeId = value as string; }
}
