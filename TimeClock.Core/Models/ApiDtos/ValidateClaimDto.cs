using System.Runtime.CompilerServices;
using TimeClock.Core.Models.EntityDtos;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.ApiDtos;

internal class ValidateClaimDto : CanJson<ValidateClaimDto>
{
    public Guid DeviceId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? Password { get; set; }
    public IEnumerable<AuthorizationClaimType> Claims { get; set; } = [];
    public PunchTypeDto PunchTypeDto { get; set; }
}
