using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;
[DebuggerDisplay("{FullNameOr}; {DefaultJobType?.Description}; {DefaultJobStep?.Description}; {UnionCode}")]
public class UserDto
{
    private IList<EquipmentDto>? _equipments;
    private IList<EquipmentsToUserDto>? _equipmentsToUsers;
    private Dictionary<DateTime, IEnumerable<PunchEntryDto>>? _punchesByDateCache;
    private PunchEntryDto? _lastPunchEntry;

    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string FullNameOr
    {
        get
        {
            string name = $"{this.FirstName} {this.LastName}".Trim();
            return string.IsNullOrWhiteSpace(name) ? (this.UserName ?? string.Empty) : name;
        }
    }
    public string? UnionCode { get; set; }
    public bool IsActive { get; set; }

    public int FailureCount { get; set; }

    public DateTime? LastActionOn { get; set; }

    public DateTime? LockedOutOn { get; set; }

    public string? PrimaryEmail { get; set; }

    public Guid? DefaultJobTypeId { get; set; }

    public Guid? DefaultJobStepId { get; set; }

    public Guid? SupervisorId { get; set; }

    public int? SupervisorJdeId { get; set; }

    public int? JdeId { get; set; }

    public bool IsAdmin { get; set; }

    public Guid DepartmentsToLocationId { get; set; }

    public JobTypeDto? DefaultJobType { get; set; }

    public JobStepDto? DefaultJobStep { get; set; }

    public UserDto? Supervisor { get; set; }

    public IList<UserDto> Subordinates { get; set; } = [];

    public IList<PunchEntryDto> PunchEntries { get; set; } = [];
    public IList<BarcodeDto> Barcodes { get; set; } = [];
    public IList<EquipmentDto> Equipments
    {
        get => this._equipments ??= [];
        set
        {
            this._equipments = value;
            if (this._equipmentsToUsers is null)
                this._equipmentsToUsers = value.Where(q => q is not null).SelectMany(q => q.EquipmentsToUsers.Where(t => t is not null)).ToList();
        }
    }
    public IList<EquipmentsToUserDto> EquipmentsToUsers
    {
        get => this._equipmentsToUsers ??= [];
        set
        {
            this._equipmentsToUsers = value;
            if (this._equipments is null)
                this._equipments = value.Where(q => q?.Equipment is not null).Select(q => q.Equipment).ToList()!;
        }
    }
    public IList<UserClaimDto> UserClaims { get; set; } = [];
    public IList<JobTypeStepToUserDto> JobTypeSteps { get; set; } = [];
    public IEnumerable<PunchEntryDto> GetPunchesByDate(DateTime date)
    {
        if (this.PunchEntries.Count < 1)
            return Enumerable.Empty<PunchEntryDto>();
        if (this._punchesByDateCache == default)
            this._punchesByDateCache = this.PunchEntries.GroupBy(p => p.DateTime.Date).ToDictionary(g => g.Key, g => g.OrderBy(q => q.DateTime.TimeOfDay).AsEnumerable());

        this._lastPunchEntry ??= this._punchesByDateCache[this._punchesByDateCache.Keys.Max()].LastOrDefault();

        return this._punchesByDateCache.ContainsKey(date.Date) ? this._punchesByDateCache[date] : Enumerable.Empty<PunchEntryDto>();
    }
    public PunchEntryDto? GetLastPunchEntry()
    {
        if (this._lastPunchEntry == null)
            this.GetPunchesByDate(DateTime.Now);
        return this._lastPunchEntry;
    }
}
