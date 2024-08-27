namespace TimeClock.Data.Models;
/// <summary>
/// Represents the beginning point at which a JobType become active for a User within a WorkPeriod
/// </summary>
public partial class WorkPeriodJobTypeStep : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    
    public DateTime ActivatedOn { get; set; }
    /// <summary>
    /// The <see cref="ActiveSince"/> ia a dependent value. It depends on the Entity (if any) which was created alongside this <see cref="WorkPeriodJobTypeStep"/>. 
    /// If the sibling Entity is a <see cref="PunchEntry"/>, then <see cref="ActiveSince"/> will reflect the DateTime of the current active stable state. 
    /// If the sibling entity is a <see cref="Equipment"/>, then <see cref="ActiveSince"/> will reflect the DateTime of the associated <see cref="EquipmentsToUser"/>. 
    /// If a relationship does not exists with either of the aforementioned Entity, then <see cref="ActiveSince"/> will reflect the DateTime of <see cref="ActivatedOn"/>. 
    /// This is because a <see cref="WorkPeriodJobTypeStep"/> will usually exist alongside one of those two Entity, but it does not have to. 
    /// When it does, that means this <see cref="WorkPeriodJobTypeStep"/> is attached to the Entity. If the DateTime for that Entity changes, this <see cref="WorkPeriodJobTypeStep"/> 
    /// must also move its <see cref="ActivatedOn"/> DateTime value to match the Entity. Only a single relationship should exist, meaning one Entity or the other. 
    /// In the scenario where both exist, <see cref="PunchEntry"/> will take precedence. The order is: <see cref="PunchEntry"/> OR <see cref="Equipment"/> OR <see cref="ActivatedOn"/>. 
    /// Due to this behavior <see cref="ActiveSince"/> cannot be set, as it is a calculated value.
    /// </summary>
    public DateTime ActiveSince { get; set; }
    public DateTime? DeactivatedOn { get; set; }
    public Guid WorkPeriodId { get; set; }
    public Guid? JobTypeId { get; set; }
    public Guid? JobStepId { get; set; }
    public Guid? PunchEntryId { get; set; }
    public Guid? EquipmentsToUserId { get; set; }
    public Guid? DeactivatedById { get; set; }
    public virtual WorkPeriod WorkPeriod { get; set; } = null!;
    public virtual JobType? JobType { get; set; } = null!;
    public virtual JobStep? JobStep { get; set; } = null!;
    public virtual PunchEntry? PunchEntry { get; set; }
    public virtual EquipmentsToUser? EquipmentsToUser { get; set; }
    public virtual User? DeactivatedBy { get; set; }
}
