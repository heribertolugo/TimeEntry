using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models;

public partial class User : IEntityModel, IReferenceJde
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string EmployeeNumber { get; set; } = null!;

    public string? UserName { get; set; } = null!;

    public bool? IsActive { get; set; }

    public int FailureCount { get; set; }

    public DateTime? LastActionOn { get; set; }

    public DateTime? LockedOutOn { get; set; }

    public string? UnionCode { get; set; }

    public string? PrimaryEmail { get; set; }

    public bool IsAdmin { get; set; }

    public int? JdeId { get; set; }

    public Guid? DefaultJobTypeId { get; set; }

    public Guid? DefaultJobStepId { get; set; }

    public Guid? DepartmentsToLocationId { get; set; }

    public Guid? SupervisorId { get; set; }

    public int? SupervisorJdeId { get; set; }

    public virtual JobType? DefaultJobType { get; set; }

    public virtual JobStep? DefaultJobStep{ get; set; }

    public virtual DepartmentsToLocation DepartmentsToLocation { get; set; } = null!;

    public virtual User? Supervisor { get; set; }

    public virtual ICollection<EquipmentsToDepartmentLocation> EquipmentsToDepartmentLocations { get; set; } = [];

    public virtual ICollection<EquipmentsToUser> EquipmentsToUsers { get; set; } = [];

    public virtual ICollection<PunchEntry> PunchEntries { get; set; } = [];
    /// <summary>
    /// The punch entries this user has created or edited. 
    /// Does not express who the punch entry is for, but rather who created it.
    /// </summary>
    public virtual ICollection<PunchEntriesHistory> PunchEntriesHistories { get; set; } = [];

    public virtual ICollection<UserClaim> UserClaims { get; set; } = [];
    public virtual ICollection<WorkPeriod> WorkPeriods { get; set; } = [];
    public virtual ICollection<User> Subordinates { get; set; } = [];
    public virtual ICollection<JobTypeStepToUser> JobTypeSteps { get; set; } = [];
    public virtual ICollection<Barcode> Barcodes { get; set; } = [];
    [NotMapped]
    object? IReferenceJde.JdeId { get => this.JdeId; set => this.JdeId = value as int?; }
}
