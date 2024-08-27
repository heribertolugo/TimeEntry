using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class UserClaimDto
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public Guid UserId { get; set; }
    public Guid AuthorizationClaimId { get; set; }

    //public UserDto? User { get; set; }
    public AuthorizationClaimDto? AuthorizationClaim { get; set; }
}
