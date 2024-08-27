namespace TimeClock.Data.Models;

public partial class EventAudit : IEntityModel
{
    public Guid Id { get; set; }

    public int RowId { get; set; }

    public int EventId { get; set; }

    public string EventName { get; set; } = string.Empty;

    public string EventDescription { get; set; } = string.Empty;

    public bool Success { get; set; }

    public string EntityType { get; set; } = string.Empty;

    public Guid EntityId { get; set; }
    /// <summary>
    /// The date and time when the event occurred. This should be in UTC format.
    /// </summary>
    public DateTime EventDate { get; set; }

    public DateTime EventDateOnly { get => this.EventDate.Date; set { } }
}
