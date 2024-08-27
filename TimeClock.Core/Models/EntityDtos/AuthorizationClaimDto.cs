using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]

namespace TimeClock.Core.Models.EntityDtos;

public class AuthorizationClaimDto
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public IList<UserClaimDto> UserClaims { get; set; } = [];
}
