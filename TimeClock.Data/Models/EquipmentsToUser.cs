using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

/// <summary>
/// Represents a relationship between Equipment and a User, 
/// where the relationship typically identifies that a User used the Equipment.
/// </summary>
public partial class EquipmentsToUser : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public DateTime? LinkedOn { get; set; }

    public DateTime? LinkedOnEffective { get; set; }

    public DateTime? UnLinkedOn { get; set; }

    public DateTime? UnLinkedOnEffective { get; set; }

    public Guid UserId { get; set; }

    public Guid EquipmentId { get; set; }

    public Guid LinkedById { get; set; }

    public Guid? UnlinkedById { get; set; }

    public Guid WorkPeriodId { get; set; }

    public Guid? JobTypeId { get; set; }

    public Guid? JobStepId { get; set; }

    public int? JdeId { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual User LinkedBy { get; set; } = null!;

    public virtual User UnlinkedBy { get; set; } = null!;

    public virtual WorkPeriod WorkPeriod { get; set; } = null!;

    public virtual JobType? JobType { get; set; }

    public virtual JobStep? JobStep { get; set; }
    [NotMapped]
    object? IReferenceJde.JdeId { get => this.JdeId; set => this.JdeId = value as int?; }
}
