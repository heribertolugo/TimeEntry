using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

public class JobType : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public string Description { get; set; } = null!;
    public bool? IsActive { get; set; }
    public string? JdeId { get; set; }
    [NotMapped]
    object? IReferenceJde.JdeId { get => this.JdeId; set => this.JdeId = value as string; }
}
