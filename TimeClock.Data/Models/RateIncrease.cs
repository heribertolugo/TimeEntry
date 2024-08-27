namespace TimeClock.Data.Models;

public class RateIncrease : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    public decimal Amount { get; set; }
    public RateIncreaseType RateIncreaseType { get; set; }
}
