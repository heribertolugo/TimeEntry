namespace TimeClock.Data.Helpers;

internal static class CommonValues
{
    public static readonly string Schema = "timeclock";
#if DEBUG
    public static readonly string JdeSchema = "TESTDTA";
    public static readonly string JdeCtlSchema = "TESTCTL";
#else

    public static readonly string JdeSchema = "PRODDTA";
    public static readonly string JdeCtlSchema = "PRODCTL";
#endif
}
