namespace TimeClock.Data.Models;

/// <summary>
/// What JobType are available to an equipment. 
/// If an equipment will have more than 1 option, the JobType, JobStep and UnionCode combination must be unique.
/// </summary>
public partial class JobTypeStepToEquipment : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    public string? UnionCode { get; set; }
    public Guid EquipmentId { get; set; }
    public virtual JobType? JobType { get; set; }
    public virtual JobStep? JobStep { get; set; }
    public virtual Equipment Equipment { get; set; } = null!;
}