using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public enum PunchTypeDto
{
    None,
    Barcode,
    Domain,
    User
}
