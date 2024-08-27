namespace TimeClock.Data.Helpers;
internal static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
    }
    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.StartOfDay().Add(new TimeSpan(0, 23, 59, 59, 999));
    }
    /// <summary>
    /// Returns the next occurrence of the provided DayOfWeek. 
    /// This is exclusive, so if Monday is passed in - the next Monday will be returned.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
    {
        int daysInWeek = 7;

        return dateTime.AddDays(daysInWeek - (int)dateTime.DayOfWeek + (int)dayOfWeek);
    }
    /// <summary>
    /// Returns the DateTime rounded to the closest value at the chosen interval
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="interval">A value representing minutes to use as a rounding interval</param>
    /// <returns></returns>
    public static DateTime RoundMinutes(this DateTime dateTime, int interval)
    {
        int newMinutes = RoundToInterval(dateTime.Minute, interval);
        int hour = dateTime.Hour;

        if (newMinutes >= 60)
        {
            hour++;
            newMinutes = 0;
        }

        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hour, newMinutes, 0);
    }
    /// <summary>
    /// Returns a number representing the number rounded to the nearest interval
    /// </summary>
    /// <param name="number"></param>
    /// <param name="interval"></param>
    /// <returns></returns>
    private static int RoundToInterval(int number, int interval)
    {
        int remainder = number % interval;
        return (number += remainder <= interval / 2 ? -remainder : interval - remainder);
    }

    public static DateOnly ToDateOnly(this DateTime dateTime)
    {
        return DateOnly.FromDateTime(dateTime);
    }
}
