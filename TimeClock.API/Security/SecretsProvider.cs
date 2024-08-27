using System.Text;
using TimeClock.Core.Models;

namespace TimeClock.Api.Security;

internal sealed class SecretsProvider(IConfiguration configuration)
{
    private static readonly string JwtIssuerKey = "TimeClockJwt:Issuer";
    private static readonly string JwtAudienceKey = "TimeClockJwt:Audience";
    private static readonly string JwtIssuerSigningKey = "TimeClockJwt:IssuerSigningKey";
    private static readonly string TimeClockConnectionUserKey = "DbUsers:timeclocksql:user";
    private static readonly string TimeClockConnectionPasswordKey = "DbUsers:timeclocksql:password";
    private static readonly string TimeClockConnectionString = "ConnectionStrings:timeclocksql";
    private static readonly string JdeConnectionUserKey = "DbUsers:jdeoracle:user";
    private static readonly string JdeConnectionPasswordKey = "DbUsers:jdeoracle:password";
    private static readonly string JdeConnectionString = "ConnectionStrings:jdeoracle";
    private static readonly string TimeEntryIntervalMinutesKey = "TimeEntryRoundingMinutes";
    private static readonly string WorkPeriodThresholdHoursKey = "WorkPeriodThresholdHours";
    private static readonly string MaxUserFailuresKey = "MaxUserFailures";
    private static readonly string MaxDeviceFailuresKey = "MaxDeviceFailures";
    private static readonly string EmailHostKey = "EmailCredentials:host";
    private static readonly string EmailPortKey = "EmailCredentials:port";
    private static readonly string EmailSslKey = "EmailCredentials:ssl";
    private static readonly string EmailUserNameKey = "EmailCredentials:email";
    private static readonly string EmailPasswordKey = "EmailCredentials:password";
    private static readonly string PunchRequestsBccKey = "PunchRequestsBccParties";
    private static readonly string LocationToJobCodeKey = "DivisionToJobCodeMap";
    private IConfiguration Config { get; set; } = configuration;

    // azure key vault: https://learn.microsoft.com/en-us/azure/key-vault/general/overview
    // azure app config: https://learn.microsoft.com/en-us/azure/azure-app-configuration/overview
    // azure key vault pricing: https://azure.microsoft.com/en-us/pricing/details/key-vault/
    public string? GetJwtIssuer() => this.Config.GetValue<string>(SecretsProvider.JwtIssuerKey);
    public string? JwtAudience => this.Config.GetValue<string>(SecretsProvider.JwtAudienceKey);
    public byte[]? GetIssuerSigningKey()
    {
        string? value = this.Config.GetValue<string>(SecretsProvider.JwtIssuerSigningKey);

        if (value is null)
            return null;

        return Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(value)));
    }
    public string? GetTimeClockConnectionString()
    {
        string? connectionString = this.Config.GetValue<string>(SecretsProvider.TimeClockConnectionString);
        string user = this.Config.GetValue<string>(SecretsProvider.TimeClockConnectionUserKey) ?? string.Empty;
        string password = this.Config.GetValue<string>(SecretsProvider.TimeClockConnectionPasswordKey) ?? string.Empty;

        if (connectionString is null)
            return null;

        return string.Format(connectionString, user, password);
    }
    public string? GetJdeConnectionString()
    {
        string? connectionString = this.Config.GetValue<string>(SecretsProvider.JdeConnectionString);
        string user = this.Config.GetValue<string>(SecretsProvider.JdeConnectionUserKey) ?? string.Empty;
        string password = this.Config.GetValue<string>(SecretsProvider.JdeConnectionPasswordKey) ?? string.Empty;

        if (connectionString is null)
            return null;

        return string.Format(connectionString, user, password);
    }
    public TimeSpan GetJwtClockScrew() => this.GetSubSectionTimeSpan("TimeClockJwt", "clockscrew");
    public TimeSpan GetJwtTokenExpiration() => this.GetSubSectionTimeSpan("TimeClockJwt", "tokenexpiration");
    public TimeSpan GetJwtRefreshExpiration() => this.GetSubSectionTimeSpan("TimeClockJwt", "refreshexpiration");
    private TimeSpan GetSubSectionTimeSpan(string sectionKey, string subsectionKey)
    {
        IConfigurationSection section = this.Config.GetSection(sectionKey);
        IConfigurationSection subsection = section.GetSection(subsectionKey);

        int days, hours, minutes, seconds;
        days = subsection.GetValue<int>(nameof(days));
        hours = subsection.GetValue<int>(nameof(hours));
        minutes = subsection.GetValue<int>(nameof(minutes));
        seconds = subsection.GetValue<int>(nameof(seconds));

        return new TimeSpan(days, hours, minutes, seconds);
    }
    public int GetTimeEntryInterval() => this.Config.GetValue<int>(SecretsProvider.TimeEntryIntervalMinutesKey);
    public int GetWorkPeriodThresholdHours() => this.Config.GetValue<int>(SecretsProvider.WorkPeriodThresholdHoursKey);
    public int GetMaxUserFailures() => this.Config.GetValue<int>(SecretsProvider.MaxUserFailuresKey);
    public int GetMaxDeviceFailures() => this.Config.GetValue<int>(SecretsProvider.MaxDeviceFailuresKey);
    public EmailConnectivity GetEmailConnectivity()
    {
        return new EmailConnectivity(
            this.Config.GetValue<string>(SecretsProvider.EmailUserNameKey) ?? string.Empty,
            this.Config.GetValue<string>(SecretsProvider.EmailHostKey) ?? string.Empty,
            this.Config.GetValue<string>(SecretsProvider.EmailPasswordKey) ?? string.Empty,
            this.Config.GetValue<bool>(SecretsProvider.EmailSslKey),
            this.Config.GetValue<int>(SecretsProvider.EmailPortKey)
            );
    }
    public string[] GetPunchRequestBccParties()
    {
        return this.Config.GetSection(SecretsProvider.PunchRequestsBccKey).Get<string[]?>() ?? [];
    }
    private KeyValuePair<string, string>[]? _locationDivisionsToJobCodes;
    public KeyValuePair<string,string>[] GetLocationDivisionsToJobCodes()
    {
        return 
            this._locationDivisionsToJobCodes ??= this.Config.GetSection(SecretsProvider.LocationToJobCodeKey).Get<KeyValuePair<string, string>[]?>()
            ?? [];
    }
}
