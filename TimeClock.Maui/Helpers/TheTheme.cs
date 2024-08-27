using TimeClock.Maui.Models;

namespace TimeClock.Maui.Helpers
{
    internal static class TheTheme
    {
        public static void SetTheme()
        {
            if (App.Current == null)
                return;
            switch (Settings.CurrentTheme)
            {
                case AppTheme.Light:
                    App.Current.UserAppTheme = AppTheme.Light;
                    break;
                case AppTheme.Dark:
                    App.Current.UserAppTheme = AppTheme.Dark;
                    break;
                default:
                    App.Current.UserAppTheme = AppTheme.Dark;
                    break;
            }

            var nav = App.Current?.MainPage as NavigationPage;

            var e = DependencyService.Get<IEnvironment>();
            if (App.Current?.RequestedTheme == AppTheme.Dark)
            {
                e?.SetStatusBarColor(Colors.Black, false);
                if (nav != null)
                {
                    nav.BarBackgroundColor = Colors.Black;
                    nav.BarTextColor = Colors.White;
                }
            }
            else
            {
                e?.SetStatusBarColor(Colors.White, true);
                if (nav != null)
                {
                    nav.BarBackgroundColor = Colors.White;
                    nav.BarTextColor = Colors.Black;
                }
            }
        }
    }
}
