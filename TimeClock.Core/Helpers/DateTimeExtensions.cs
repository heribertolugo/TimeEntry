namespace TimeClock.Core;

public static class DateTimeExtensions
{
    public static DateTime StartOfDay(this DateTime dateTime)
    {
        return dateTime.Date;// new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
    }
    public static DateTime EndOfDay(this DateTime dateTime)
    {
        return dateTime.StartOfDay().Add(new TimeSpan(0, 23, 59, 59, 999));
    }
    /// <summary>
    /// Determines whether the specified DateTime is within the dates specified in from and to, inclusive. 
    /// from uses the StartOfDay and to uses EndOfDay, the time in from and to are not considered.
    /// </summary>
    /// <param name="datetime"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static bool IsInDateRange(this DateTime datetime, DateTime from, DateTime to)
    {
        return datetime >= from.StartOfDay() && datetime <= to.EndOfDay();
    }
    /// <summary>
    /// Gets the DateTime that corresponds to the DateTime considered to be the beginning of the week for the DateTime provided. 
    /// The beginning of the week is the first Sunday before the provided DateTime, unless the provided DateTime is Sunday. 
    /// In which case, the same DateTime is returned.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime BeginningOfWeek(this DateTime dateTime)
    {
        return dateTime.AddDays((int)dateTime.DayOfWeek * -1);
    }
    /// <summary>
    /// Gets the DateTime that corresponds to the DateTime considered to be the end of the week for the DateTime provided. 
    /// The ending of the week is the Saturday after the provided DateTime, unless the provided DateTime is Saturday. 
    /// In which case, the same DateTime is returned.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static DateTime EndOfWeek(this DateTime dateTime)
    {
        return dateTime.AddDays(6 - (int)dateTime.DayOfWeek);
    }
    /// <summary>
    /// Allows iterating through dates by day (24 hours) starting from the current, up to the until Date specified. 
    /// If no until Date is specified, DateTime.Max is used.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="until"></param>
    /// <returns></returns>
    public static IEnumerable<DateTime> UntilDate(this DateTime dateTime, DateTime? until = null)
    {
        DateTime current = dateTime;
        if (!until.HasValue)
            until = DateTime.MaxValue.AddDays(-1);

        do
        {
            yield return current;

            current = current.AddDays(1);
        } while (current.Date <= until.Value.Date);
    }
    public static IEnumerable<DateTime> Until(this DateTime dateTime)
    {
        DateTime current = dateTime;

        do
        {
            yield return current;

            current = current.AddDays(1);
        } while (current <= DateTime.MaxValue.AddDays(-1));
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
        int skips = daysInWeek - (int)dateTime.DayOfWeek + (int)dayOfWeek;
        if (skips > daysInWeek) skips -= daysInWeek;

        return dateTime.AddDays(skips);
    }
    /// <summary>
    /// <inheritdoc cref="NextDayOfWeek(DateTime, DayOfWeek)" />
    /// </summary>
    /// <param name="date"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateOnly NextDayOfWeek(this DateOnly date, DayOfWeek dayOfWeek)
    {
        int daysInWeek = 7;
        int skips = daysInWeek - (int)date.DayOfWeek + (int)dayOfWeek;
        if (skips > daysInWeek) skips -= daysInWeek;

        return date.AddDays(skips);
    }
    /// <summary>
    /// Returns the previous occurrence of the provided DayOfWeek.
    /// This is exclusive, so if Monday is passed in - the previous Monday will be returned.
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="dayOfWeek"></param>
    /// <returns></returns>
    public static DateTime PreviousDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
    {
        int daysInWeek = 7;
        int skips = (int)dateTime.DayOfWeek - daysInWeek;
        if (skips < 0) skips += daysInWeek;
        DateTime result = dateTime.AddDays((skips * -1) + 1); // negate the value and add offset
        if (result.Date == dateTime.Date)
            result = result.AddDays(-daysInWeek);
        return result;
    }

    public static DateTime AtTimeOfDay(this DateTime dateTime, int hours, int minutes = 0, int seconds = 0)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, hours, minutes, seconds);
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
    public static TimeSpan RoundMinutes(this TimeSpan timeSpan, int interval)
    {
        int newMinutes = RoundToInterval(timeSpan.Minutes, interval);
        int hour = timeSpan.Hours;

        if (newMinutes >= 60)
        {
            hour++;
            newMinutes = 0;
        }

        return new TimeSpan(hour, newMinutes, 0);
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
    /// <summary>
    /// Determines if a Date's cutoff occurs before the current DateTime. 
    /// The Date's cutoff is determined by the next occurrence of dayOfWeek at the specified cutoffHour.
    /// </summary>
    /// <param name="date">The Date which has a cutoff at the next provided dayOfWeek</param>
    /// <param name="dayOfWeek">The DayOfWeek which designates the Date's cutoff Date.</param>
    /// <param name="cutoffHour">The hour (in 24 hour format) which specifies the hour of the cutoff.</param>
    /// <returns>a bool which represents whether that Date provided is past its cutoff point in time.</returns>
    public static bool IsBeforeCutoff(this DateTime date, DayOfWeek dayOfWeek = DayOfWeek.Monday, int cutoffHour = 9)
    {
        return date.AtTimeOfDay(0).NextDayOfWeek(dayOfWeek).AtTimeOfDay(cutoffHour) > DateTime.Now;
    }

    public static TimeOnly ToTimeOnly(this DateTime dateTime)
    {
        return TimeOnly.FromDateTime(dateTime);
    }

    public static DateOnly ToDateOnly(this DateTime date)
    {
        return DateOnly.FromDateTime(date);
    }
}
