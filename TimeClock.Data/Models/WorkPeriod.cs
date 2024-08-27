using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

/*
 * advise user NOT to update work period in progress
 * (please do not edit a work period until user has clocked out for the day. you may add a new work period 
 * for special time)
 * to add special time, create new work period
 * when calculating punches to update work period, first get all related work periods and subtract from punch total
 * then update work period connected to punches
 */
public partial class WorkPeriod : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public double HoursWorked { get; set; }
    public DateOnly WorkDate { get; set; }
    public Guid UserId { get; set; }
    public WorkPeriodPurpose Purpose { get; set; }
    public DateOnly PayPeriodEnd { get; set; }
    public bool? IsPreviousMissingPunch { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<WorkPeriodStatusHistory> WorkPeriodStatusHistories { get; set; } = [];
    public virtual ICollection<PunchEntry> PunchEntries { get; set; } = [];
    public virtual ICollection<EquipmentsToUser> EquipmentsToUsers { get; set; } = [];
    [NotMapped]
    public virtual ICollection<WorkPeriodJobTypeStep> WorkPeriodJobTypeSteps { get; set; } = [];
}
