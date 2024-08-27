namespace TimeClock.Data.Models;

public partial class UnregisteredDevice : IEntityModel
{
    public Guid Id { get; set; }

    public int RowId { get; set; }

    public required string Name { get; set; } = null!;

    public string? RefreshToken { get; set; } 
}
