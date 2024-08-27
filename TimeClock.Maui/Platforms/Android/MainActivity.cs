using Android.App;
using Android.Content.PM;
using Android.OS;

namespace TimeClock.Maui
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            var app = (App.Current as App);
            if (Helpers.Settings.ActivityRedirectActive && app is not null)
                app.LastActivity = DateTime.Now;
            // for iOS version see:
            // https://stackoverflow.com/a/73837088/6368401
            // https://www.pranavkhandelwal.com/blog/2015/11/1/detecting-user-inactivityidle-time-for-xamarinios
        }
    }

}
