namespace TimeClock.Data.Models;

public partial class Device : IEntityModel
{
    public Guid Id { get; set; }

    public int RowId {  get; set; }

    public string Name { get; set; } = null!;

    public Guid DepartmentsToLocationsId { get; set; }

    public bool? IsActive { get; set; }
    /// <summary>
    /// Running count of consecutive failed logins or punches on this device.
    /// </summary>
    public int FailureCount { get; set; }
    /// <summary>
    /// The last time any action was performed on this device (punch-in/out, login, etc.)
    /// </summary>
    public DateTime? LastActionOn { get; set; }

    public DateTime? LockedOutOn { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiration { get; set; }

    public DateTime? RefreshTokenIssuedOn { get; set; }
    /// <summary>
    /// Whether this device is meant to be used by multiple employees
    /// </summary>
    public bool IsPublic { get; set; }

    public Guid ConfiguredById { get; set; }

    public virtual DepartmentsToLocation DepartmentsToLocations { get; set; } = null!;
    /// <summary>
    /// The User responsible for the initial configuration of the TimeClock app the this device.
    /// </summary>
    public virtual User ConfiguredBy { get; set; } = null!;

    public virtual ICollection<PunchEntriesHistory> PunchEntriesHistories { get; set; } = new List<PunchEntriesHistory>();
}
