using MetroLog.Maui;
using Microsoft.Extensions.Logging;
using TimeClock.Maui.Helpers;

namespace TimeClock.Maui
{
    public partial class App : Application
    {
        public App()
        {
            // first thing, setup global event handler so we can log application crash exceptions
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                ILogger<App>? logger = ServiceHelper.GetLoggerService<App>(false);
                logger?.LogCritical(new EventId(1, "UnhandledException"), e?.ExceptionObject as Exception, "{sender}", s?.GetType().Name);
            };

#if WINDOWS
                Helpers.CleanExit.Apply();
#endif
            this.InitializeComponent();

            base.MainPage = new AppShell();

            // let MetroLogger know our navigation/main page so it can push modal to show log page
            LogController.InitializeNavigation(
                page => this.MainPage!.Navigation.PushModalAsync(page),
                () => this.MainPage!.Navigation.PopModalAsync());
        }

        internal DateTime LastActivity { get; set; } = DateTime.Now;
        internal Tuple<int, IUiPageMeta> ScreenChangeSettings = new(Settings.IdleScreenSeconds, Settings.IdleScreen);
        internal void NonActivityRedirect()
        {
            if (DateTime.Now > this.LastActivity.AddSeconds(this.ScreenChangeSettings.Item1))
            {
                // set date far, since we dont want to redirect if we are at the redirect target
                this.LastActivity = DateTime.MaxValue.AddDays(-1);
                Application.Current?.Dispatcher.Dispatch(() =>
                {
                    Shell.Current.GoToAsync(this.ScreenChangeSettings.Item2.Path, true);
                });
            }
        }

#if WINDOWS
// set min size for app window when on Windows platform
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        window.MinimumWidth = 1170;
        window.MinimumHeight = 750;

        return window;
    }
#endif
    }
}
