using Microsoft.Extensions.Configuration;
using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TimeClock.Core.Models;
using TimeClock.JdeSync.Helpers;
using TimeClock.JdeSync.Services;

internal class Program
{
    #region Private Properties
    private static ILogger Logger { get; set; } = null!;
    private static CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource(TimeSpan.FromHours(1));
    private static CancellationToken CancellationToken { get; } = CancellationTokenSource.Token;
    private static string AppHasRunFileName = "NotFirstRun";
    #endregion Private Properties

    public static async Task Main(string[] args)
    {
        ConfigurationBuilder builder = new();
        BuildConfig(builder);
        IConfigurationRoot config = builder.Build();
        Stopwatch stopwatch = Stopwatch.StartNew();
        Stopwatch localWatch = new();

        UpdateLogFilePath(config);

        Logger = Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config.GetSection("Logging"))
            .CreateLogger();

        Logger.LogStart();
        string[] cleanArgs = args.Select(a => a.Trim().TrimStart('-', '\\', '/').ToLower()).ToArray();
        string[] validCommands = ["sync", "sync:mem", "timeentry", "routine"];
        string firstRunFilePath = Path.Combine(AppContext.BaseDirectory, Program.AppHasRunFileName);
        bool isFirstRun = !File.Exists(firstRunFilePath);

        if (cleanArgs.Length < 1 || !validCommands.Intersect(cleanArgs).Any())
        {
            Console.WriteLine("To perform actions please run with one or more of the following command line parameters:");
            Console.WriteLine();
            Console.WriteLine("-sync");
            Console.WriteLine("This will export data from JDE database into TimeClock database and synchronize that data.");
            Console.WriteLine("You can use -sync:mem instead to shrink the size of the running process in RAM. Will run slower, but smaller.");
            Console.WriteLine();
            Console.WriteLine("-timeentry");
            Console.WriteLine("This will create and insert time entries into JDE.");
            Console.WriteLine();
            Console.WriteLine("-routine");
            Console.WriteLine("This will run the following routine tasks:");
            Console.WriteLine("* Email employees for missed punches.");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            Program.Logger.Warning("Application ran with no arguments. Now closing.");
            return;
        }

        try
        {
            // force allow null. we need to throw if null, since we cant continue without a connection string
            string tcConnectionString = config.GetConnectionString(CommonValues.TimeClockConnectionStringName)!;
            string jdeConnectionString = config.GetConnectionString(CommonValues.JdeConnectionStringName)!;
            //TimeEntrySchedule timeEntrySchedule = config.GetSection(nameof(TimeEntrySchedule)).Get<TimeEntrySchedule>()!;

            if (cleanArgs.Contains("sync") || cleanArgs.Contains("sync:mem"))
            {
                char[]? empStatusCodes = config.GetSection("ActiveEmployeeStatusCodes").Get<char[]>();
                await Program.SyncData(tcConnectionString, jdeConnectionString, !isFirstRun, cleanArgs.Contains("sync"), empStatusCodes, 
                    localWatch, Program.Logger);
            }

            if (cleanArgs.Contains("timeentry"))
            {
                localWatch.Restart();
                await Program.TimeEntriesToJde(tcConnectionString, jdeConnectionString, stopwatch, Logger, CancellationToken);
                Logger.Warning("timeentry processing took {0}", localWatch.Elapsed.ToString());
                localWatch.Stop();
            }

            if (cleanArgs.Contains("routine"))
            {
                EmailConnectivity connectivity = new(
                    config.GetValue<string>(CommonValues.EmailUserKey) ?? string.Empty,
                    config.GetValue<string>(CommonValues.EmailServerKey) ?? string.Empty,
                    config.GetValue<string>(CommonValues.EmailPasswordKey) ?? string.Empty,
                    config.GetValue<bool>(CommonValues.EmailUseSslKey),
                    config.GetValue<int>(CommonValues.EmailPortKey));
                string[] bcc = config.GetValue<string>(CommonValues.MissedPunchesBccKey)?.Split(';', StringSplitOptions.TrimEntries|StringSplitOptions.RemoveEmptyEntries) ?? [];

                await Program.RoutineActions(tcConnectionString, config.GetValue<string>(CommonValues.FallbackEmailKey) ?? string.Empty, bcc, connectivity, localWatch, Program.Logger, Program.CancellationTokenSource.Token);
            }

            stopwatch.Stop();
            Logger.LogEnd();
            Logger.Warning("processing took {0}", stopwatch.Elapsed.ToString());

            if (isFirstRun)
                using (File.Create(firstRunFilePath)) { };

#if DEBUG
            Console.ReadLine();
#endif

        }
        catch (Exception ex)
        {
            Logger.Error(ex, "@Exception");
        }
    }

    internal static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    private static async Task SyncData(string tcConnectionString, string jdeConnectionString, bool filterLoadedEmployeesToActive, bool bulkSync, char[]? empStatusCodes, Stopwatch stopwatch, ILogger logger)
    {
        stopwatch.Restart();
        JdeToTcSync jdeToTcSync = new(tcConnectionString, jdeConnectionString, logger, CancellationTokenSource);
        if (empStatusCodes is not null)
            jdeToTcSync.SetActiveEmployeeStatusCodes(empStatusCodes);
        jdeToTcSync.FilterEmployeesToActive = filterLoadedEmployeesToActive;
        await jdeToTcSync.Start(bulkSync);
        jdeToTcSync.Dispose();
        stopwatch.Stop();
        logger.Warning("sync processing took {0}", stopwatch.Elapsed.ToString());
    }
    private static async Task RoutineActions(string tcConnectionString, string fallbackEmail, string[] bcc, EmailConnectivity connectivity, Stopwatch stopwatch, ILogger logger, 
        CancellationToken cancellationToken = default)
    {
        stopwatch.Restart();
        EmailerService emailerService = new(tcConnectionString, connectivity, Logger,
            fallbackEmail, bcc, cancellationToken);
        await emailerService.Start();
        emailerService.Dispose();
        stopwatch.Stop();
        logger.Warning("emailer processing took {0}", stopwatch.Elapsed.ToString());
    }
    private static async Task TimeEntriesToJde(string tcConnectionString, string jdeConnectionString, Stopwatch stopwatch, ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var etl = new TcToJdeEtl(tcConnectionString, jdeConnectionString, logger, cancellationToken);
        await etl.Start();
    }

    // this is thanks to Ivan @ https://stackoverflow.com/a/75988212/6368401
    private static void UpdateLogFilePath(IConfigurationRoot configuration)
    {
        // A configuration key pattern we are going to look for
        string pattern = "^Logging:Serilog:WriteTo.*Args:path$";

        foreach (var kvp in configuration.GetSection("Logging").AsEnumerable())
        {
            if (Regex.IsMatch(kvp.Key, pattern, RegexOptions.IgnoreCase)
                && !string.IsNullOrEmpty(kvp.Value))
            {
                // Format a file path in the logger configuration
                configuration[kvp.Key] = string.Format(kvp.Value, DateTime.Now);
            }
        }
    }
}
