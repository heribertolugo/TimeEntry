
namespace TimeClock.Data.Models;

public partial class UserClaim : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public Guid UserId { get; set; }
    public Guid AuthorizationClaimId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual AuthorizationClaim AuthorizationClaim { get; set; } = null!;
}
