namespace TimeClock.Core.Models;

public readonly struct DateRange(DateTime start, DateTime end)
{
    public readonly static DateRange Default = new DateRange(new DateTime(1753, 1, 1), new DateTime(9999, 12, 31));

    public DateTime Start { get; init; } = start;
    public DateTime End { get; init; } = end;
}
