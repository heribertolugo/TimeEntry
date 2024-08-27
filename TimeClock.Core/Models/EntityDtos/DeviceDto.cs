using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class DeviceDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid DepartmentsToLocationsId { get; set; }
    public bool IsActive { get; set; }

    public int FailureCount { get; set; }

    public DateTime? LastActionOn { get; set; }

    public DateTime? LockedOutOn { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiration { get; set; }
    public DateTime? RefreshTokenIssuedOn { get; set; }
    public bool IsPublic { get; set; }
    public Guid ConfiguredById { get; set; }
    public DepartmentsToLocationDto? DepartmentsToLocations { get; set; }
    public UserDto? ConfiguredBy { get; set; }
    public IList<PunchEntryDto> PunchEntries { get; set; } = [];
}
