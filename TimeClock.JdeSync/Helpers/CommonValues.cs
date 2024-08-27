namespace TimeClock.JdeSync.Helpers;
internal static class CommonValues
{
    /// <summary>
    /// Primary Company ID within JDE | 00001
    /// </summary>
    public static readonly string CompanyId = "00001";
    /// <summary>
    /// Starting ID used by TimeClock Entities which do not exist in JDE. 
    /// If another ID is needed, it should use an increment of this ID in a negative direction. 
    /// This can be changed to 0 if needed, but code in SkipNonJdeId would need to be uncommented, 
    /// and tested. As it was never fully tested.
    /// </summary>
    public static readonly string NonJdeId = "-1";
    /// <summary>
    /// <inheritdoc cref="NonJdeId"/>
    /// </summary>
    public static readonly int NonJdeIdInt = 0;
    /// <summary>
    /// Connection string for TimeClock database
    /// </summary>
    public static readonly string TimeClockConnectionStringName = "timeclocksql";
    /// <summary>
    /// Connection string for Super Admin in TimeClock database
    /// </summary>
    public static readonly string TimeClockSaConnectionStringName = "timeclocksqlSA";
    /// <summary>
    /// Connection string for JDE database
    /// </summary>
    public static readonly string JdeConnectionStringName = "jdeoracle";
    public static readonly string FallbackEmailKey = "FallbackEmail";
    public static readonly string EmailUserKey = "EmailConnection:user";
    public static readonly string EmailServerKey = "EmailConnection:server";
    public static readonly string EmailPasswordKey = "EmailConnection:password";
    public static readonly string EmailUseSslKey = "EmailConnection:ssl";
    public static readonly string EmailPortKey = "EmailConnection:port";
    public static readonly string MissedPunchesBccKey = "MissedPunchesBcc";
}
