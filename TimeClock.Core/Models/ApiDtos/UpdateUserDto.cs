using System.Runtime.CompilerServices;
using TimeClock.Core.Models.EntityDtos;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.ApiDtos;

internal class UpdateUserDto : CanJson<UpdateUserDto>
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
    public Guid ActionById { get; set; }
    /// <summary>
    /// A null value signifies that <see cref="Barcode"/> should not be updated
    /// </summary>
    public string? Barcode { get; set; }
    public string? Username { get; set; }
    public Guid[]? AllowedJobTypes { get; set; }
    public Guid[]? AllowedJobSteps { get; set; }
    public IList<JobTypeStepToUserDto> JobTypeSteps { get; set; } = [];
}
