namespace TimeClock.Data.Models
{
    public partial class AuthorizationClaim : IEntityModel
    {
        public Guid Id { get; set; }
        public int RowId { get; set; }
        public required string Type { get; set; }
        public required string Value { get; set; }

        public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();
    }
}
