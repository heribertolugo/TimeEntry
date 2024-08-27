namespace TimeClock.Data.Models;

/// <summary>
/// JobType that a User is allowed to use
/// </summary>
public partial class JobTypeStepToUser : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public bool IsActive { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    public Guid UserId { get; set; }
    public virtual JobType? JobType { get; set; } = null!;
    public virtual JobStep? JobStep { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
