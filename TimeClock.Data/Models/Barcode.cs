namespace TimeClock.Data.Models;

public partial class Barcode : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public required string Value { get; set; }
    public DateTime? ActivatedOn { get; set; }
    public DateTime? DeactivatedOn { get; set; }
    public Guid UserId { get; set; }
    public Guid? DeactivatedById { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual User? DeactivatedBy { get; set; }
}
