namespace TimeClock.JdeSync.Helpers;
internal static class DateTimeExtensions
{
    /// <summary>
    /// Gets a value representing the century of the date provided
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public static int GetCentury(this DateTime date)
    {
        int year = date.Year;
        return year % 100 == 0 ? year / 100 : (year / 100) + 1;
    }

    /// <summary>
    /// Converts a date to special Julian date format used by JDE.
    /// </summary>
    /// <param name="date"></param>
    /// <returns>An integer value representing the Date portion of a DateTime as a JDE Julian date</returns>
    /// <remarks>The returned Julian date is not the standard Julian date</remarks>
    /// <exception cref="ArgumentException">An exception is thrown if year is prior to 1900</exception>
    public static int ToJdeDate(this DateTime date)
    {
        int beginningOfTime = 1900;
        if (date.Year < beginningOfTime) 
            throw new ArgumentException($"Date is invalid. Must be after {beginningOfTime}");
        int control = (int)(date.Year / 1000) * 1000;
        int century = (int)(control - beginningOfTime) / 100 * 100000;
        int year = (date.Year - control) * 1000; //date.Year % 1000 * 1000; // modulo is slower
        int days = (int)date.Subtract(new DateTime(date.Year - 1, 12, 31)).TotalDays;
        return century + year + days;
    }
    public static int ToJdeDate(this DateOnly date)
    {
        return date.ToDateTime(default).ToJdeDate();
    }

    /// <summary>
    /// Converts an integer value representing a JDE Julian date to a standard DateTime
    /// </summary>
    /// <param name="jdeDate"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException">An exception is thrown if value is greater than 8099365 or less than 1001</exception>
    public static DateTime JdeDateToStandard(this int jdeDate)
    {
        if (jdeDate > 8099365 || jdeDate < 1001) // DateTime max value and JDE min value
            throw new ArgumentOutOfRangeException("integer",$"Value was out of range. Must be between 1001 and 8099365.");
        int beginningOfTime = 1900;
        int control = (int)(jdeDate / 100000) * 100;
        int year = control + beginningOfTime;
        jdeDate -= control * 1000;
        year += (int)jdeDate / 1000;
        jdeDate -= (int)(jdeDate / 1000) * 1000;
        return new DateTime(year - 1, 12, 31).AddDays(jdeDate);
    }

    /// <summary>
    /// Converts a <see cref="TimeSpan"/> to a 5 or 6 digit (hhmmss) <see cref="int"/> representing JDE Julian time hmm
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static int ToJdeTime(this TimeSpan time)
    {
        return (time.Hours * 10000) + (time.Minutes * 100) + (time.Seconds);
    }

    /// <summary>
    /// Converts an <see cref="int"/> representing a 5 or 6 digit (hhmmss) JDE Julian time into a <see cref="TimeSpan"/>
    /// </summary>
    /// <param name="jdeTime">4 digit JDE Julian time</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">An exception is thrown if hours are greater than 24 or minutes are greater than 59.</exception>
    public static TimeSpan JdeTimeToStandard(this int jdeTime)
    {
        int h = jdeTime / 10000;
        int m = jdeTime - (h * 10000);
        int s = m - (m / 100 * 100);
        m /= 100;
        if (h > 24 || m > 59 || s > 59) throw new ArgumentException("JDE time provided is invalid");
        return new TimeSpan(h, m, s);
    }
}
