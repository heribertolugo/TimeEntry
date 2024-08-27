using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class UnregisteredDeviceDto
{
    public Guid Id { get; set; }

    public int RowId { get; set; }

    public string Name { get; set; } = null!;

    public string? RefreshToken { get; set; }
}
