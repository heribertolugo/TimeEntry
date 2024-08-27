using CommunityToolkit.Maui.Alerts;

namespace TimeClock.Maui.Helpers
{
    internal class UiHelpers
    {
        public static async Task ShowToast(string message, int cancelDelay = 2000)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(cancelDelay);
            var toast = Toast.Make(message, CommunityToolkit.Maui.Core.ToastDuration.Long);
            try
            {
                await toast.Show(cancellationTokenSource.Token);
            }catch(OperationCanceledException) { }
        }

        public static Task ShowOkAlert(string message, string title = "OOPs")
        {
            if (App.Current?.MainPage is null)
                throw new InvalidOperationException(CommonValues.AppCurInac);
            return App.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        public static Task<bool> ShowYesNoAlert(string message, string title, string yesText = "yes", string noText = "no")
        {
            if (App.Current?.MainPage is null)
                throw new InvalidOperationException(CommonValues.AppCurInac);
            return App.Current.MainPage.DisplayAlert(title, message, yesText, noText);
        }

        public static async Task<bool?> ShowYesNoCancel(string title, string yesText = "yes", string noText = "no", string? cancelText = "")
        {
            if (App.Current?.MainPage is null)
                throw new InvalidOperationException(CommonValues.AppCurInac);
            var result = await App.Current.MainPage.DisplayActionSheet(title, cancelText, null, [ yesText, noText]);

            return result == cancelText ? null : result == yesText;
        }
    }
}
