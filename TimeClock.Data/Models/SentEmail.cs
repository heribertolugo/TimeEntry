namespace TimeClock.Data.Models;
public partial class SentEmail : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public DateTime SentOn { get; set; }
    public string SentTo { get; set; } = null!;
    public string Signature { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}
