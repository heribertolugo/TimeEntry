using CommunityToolkit.Maui;
using MetroLog.MicrosoftExtensions;
using MetroLog.Operators;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using System.Diagnostics;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels;
using TimeClock.Maui.ViewModels.AdminSection;
using TimeClock.Maui.ViewModels.HistorySection;
using TimeClock.Maui.ViewModels.Shared;
using TimeClock.Maui.Views.AdminSection;
using TimeClock.Maui.Views.HistorySection;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", CommonValues.OpenSansRegularFont);
                fonts.AddFont("OpenSans-Semibold.ttf", CommonValues.OpenSansSemiboldFont);
                fonts.AddFont("Font Awesome 6 Free-Solid-900.otf", CommonValues.FaSolidFont);
                fonts.AddFont("Font Awesome 6 Free-Regular-400.otf", CommonValues.FaRegularFont);
                fonts.AddFont("Font Awesome 6 Brands-Regular-400.otf", "FaBrands");
            });

        builder.ConfigureMauiHandlers(handlers =>
        {
#if WINDOWS
            SwitchHandler.Mapper.AppendToMapping("Custom", (h, _) =>
            {
                h.PlatformView.OffContent = string.Empty;
                h.PlatformView.OnContent = string.Empty;

                h.PlatformView.MinWidth = 0;
            });
#endif
        });

        RegisterLoggers(builder);
        RegisterServices(builder);
        RegisterViewModels(builder);
        FullScreenMode(builder);
        if (Settings.ActivityRedirectActive)
            ListenToMessageEventsWindows(builder);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    private static void RegisterLoggers(MauiAppBuilder builder)
    {
        builder.Logging
            .SetMinimumLevel(LogLevel.Trace)
#if DEBUG
            .AddTraceLogger(
                options =>
                {
                    options.MinLevel = LogLevel.Trace;
                    options.MaxLevel = LogLevel.Critical;
                }) // Will write to the Debug Output
#endif
            .AddInMemoryLogger(
                options =>
                {
                    options.MaxLines = 1024;
                    options.MinLevel = LogLevel.Debug;
                    options.MaxLevel = LogLevel.Critical;
                })
//#if RELEASE
            .AddStreamingFileLogger(
                options =>
                {
                    options.MinLevel = LogLevel.Error;
                    options.MaxLevel = LogLevel.Critical;
                    options.RetainDays = 2;
                    options.FolderPath = Path.Combine(FileSystem.CacheDirectory, "AppLogs"); // %LOCALAPPDATA%\Packages\com.company.timeclock.maui_9zz4h110yvjzm\LocalCache
                })
//#endif
            .AddConsoleLogger(
                options =>
                {
                    options.MinLevel = LogLevel.Information;
                    options.MaxLevel = LogLevel.Critical;
                }); // Will write to the Console Output (logcat for android)

        builder.Services.AddSingleton(LogOperatorRetriever.Instance);
    }

    private static void RegisterViewModels(MauiAppBuilder builder)
    {
        builder.Services.AddTransient<RegistrationViewModel>();
        builder.Services.AddTransient<PunchHistoryViewModel>();
        builder.Services.AddTransient<EquipmentHistoryViewModel>();
        builder.Services.AddTransient<EditPunchViewModel>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<PunchSummaryViewModel>();
        builder.Services.AddTransient<ManagePunchesViewModel>();
        builder.Services.AddTransient<EditEntryViewModel>();
        builder.Services.AddTransient<ManageEmployeesViewModel>();
        builder.Services.AddTransient<ConfigurationsViewModel>();
        builder.Services.AddTransient<ManageRequestsViewModel>();
    }

    private static void RegisterServices(MauiAppBuilder builder)
    {
        builder.Services.AddHttpClient();
        // IPopupService specific to the provided popup view and view model
        builder.Services.AddTransientPopup<EditPunch, EditPunchViewModel>();
        builder.Services.AddTransientPopup<EditEntryPopup, EditEntryViewModel>();
        builder.Services.AddTransientPopup<Credentials, CredentialsViewModel>();
        // service to connect to our main API
        builder.Services.AddSingleton<TimeClockApiAccessSettings>();
        builder.Services.AddSingleton<ITimeClockApiAccessService, TimeClockApiAccessService>();// (s =>
            //s.ResolveWith<TimeClockApiAccessService>(GetTimeClockApiAccessSettings()));
    }

    private static void FullScreenMode(MauiAppBuilder builder)
    {
        //FullScreenModeWinPlatform(builder);
    }

    private static void FullScreenModeWinPlatform(MauiAppBuilder builder)
    {
#if WINDOWS
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windowsLifecycleBuilder =>
            {
                windowsLifecycleBuilder.OnWindowCreated(window =>
                {
                    window.ExtendsContentIntoTitleBar = false;
                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(handle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(id);
                    switch (appWindow.Presenter)
                    {
                        case Microsoft.UI.Windowing.OverlappedPresenter overlappedPresenter:
                            //overlappedPresenter.SetBorderAndTitleBar(false, false);
                            overlappedPresenter.Maximize();
                            break;
                    }
                });
            });
        });
#endif
    }

    //private static TimeClockApiAccessSettings GetTimeClockApiAccessSettings()
    //{
    //    return new TimeClockApiAccessSettings(Settings.ApiEndpoint, "", "", Settings.DeviceId);
    //}

    private static IEnumerable<uint> ActivityMessages = new List<uint>()
    {
        Convert.ToUInt32("0x0086", 16), //WM_NCACTIVATE 134
        Convert.ToUInt32("0x0020", 16), //WM_SETCURSOR 32
        Convert.ToUInt32("0x0021", 16), //WM_MOUSEACTIVATE 33
        Convert.ToUInt32("0x0200", 16), //WM_MOUSEMOVE 512
        Convert.ToUInt32("0x0216", 16), //WM_MOVING 534
        Convert.ToUInt32("0x0005", 16), //WM_SIZE 5
        Convert.ToUInt32("0x0101", 16), //WM_KEYUP 257
    };
    private static void ListenToMessageEventsWindows(MauiAppBuilder builder)
    {
#if WINDOWS
        
        builder.ConfigureLifecycleEvents(events =>
         {
             events
                 .AddWindows(windows => windows
                     .OnPlatformMessage((window, args) =>
                     {
                         if (ActivityMessages.Contains(args.MessageId))
                         {
                             Debug.WriteLine($"WinMsg: {args.MessageId}");
                             var app = (App.Current as App);
                             if (app is not null)
                                app.LastActivity = DateTime.Now;
                         }
                     }));
         });
#endif
    }
}
