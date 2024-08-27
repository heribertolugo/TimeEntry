using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class BarcodeDto
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public string Value { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    public UserDto? User { get; set; }
}
