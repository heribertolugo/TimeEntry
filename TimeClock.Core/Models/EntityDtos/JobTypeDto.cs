using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.EntityDtos;
public class JobTypeDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public bool? IsActive { get; set; }
    public string? JdeId { get; set; }
}
