using TimeClock.Data.Models;

namespace TimeClock.Data.Repository;

public interface IAuthorizationClaimsRepository : IDataRepository<AuthorizationClaim>
{

}

public class AuthorizationClaimsRepository : DataRepository<AuthorizationClaim>, IAuthorizationClaimsRepository
{
    public AuthorizationClaimsRepository(TimeClockContext context) : base(context) { }
}

public class AuthorizationClaimsDefinition
{
    private static List<AuthorizationClaimsDefinition> AuthorizationClaimsDefinitions { get; set; } = [];

    public static readonly AuthorizationClaimsDefinition CanSelectEquipment = new ("CanSelectEquipment", "Can Select Equipment", new Guid("59ddf3ba-7f8a-4ff9-b7dc-cab50abb0c99"));
    public static readonly AuthorizationClaimsDefinition CanViewOthersPunches = new ("CanViewOthersPunches", "Can View Others Punches", new Guid("4e43c124-2e61-40bd-a8c2-222c816a519e"));
    public static readonly AuthorizationClaimsDefinition CanEditOthersPunches = new ("CanEditOthersPunches", "Can Edit Others Punches", new Guid("570a8132-f550-4bf9-b757-9192f0ff3a49"));
    public static readonly AuthorizationClaimsDefinition CanConfigureApp = new ("CanConfigureApp", "Can Configure App", new Guid("46e16cb7-0939-457f-832e-b39c25ffd196"));
    public static readonly AuthorizationClaimsDefinition CanCreateEmployee = new ("CanCreateEmployee", "Can Create Employee", new Guid("8bc49e7a-ba30-4bf9-8a52-f73b69da7ae0"));

    private AuthorizationClaimsDefinition(string type, string value, Guid guid)
    {
        this.Type = type;
        this.Value = value;
        this.Id = guid;
        AuthorizationClaimsDefinition.AuthorizationClaimsDefinitions.Add(this);
        this.RowId = AuthorizationClaimsDefinition.AuthorizationClaimsDefinitions.Count;
    }
    public string Type { get; init; }
    public string Value { get; init; }
    public Guid Id { get; init; }
    public int RowId { get; init; }

    public static IEnumerable<AuthorizationClaimsDefinition> Definitions => AuthorizationClaimsDefinition.AuthorizationClaimsDefinitions;
}
