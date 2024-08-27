namespace TimeClock.Data.Models;

public class SpecialPay : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }

    public Guid? HolidayId { get; set; }
    public Guid? SaturdayId { get; set; }
    public Guid? SundayId { get; set; }

    public virtual RateIncrease? Holiday { get; set; }
    public virtual RateIncrease? Saturday { get; set; }
    public virtual RateIncrease? Sunday { get; set; }
}
