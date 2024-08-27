using System.Collections.Concurrent;
using System.Net;
using TimeClock.Core.Models.EntityDtos;
using TimeClock.Maui.Controls;
using TimeClock.Maui.Helpers;
using TimeClock.Maui.ViewModels;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views;

public partial class Main : BaseContentPage
{
    private MainViewModel _viewModel;
    private ConcurrentDictionary<DoubleLabel, bool> _toasts;
    private ConcurrentDictionary<ToastSeverityLevel, Color> _toastsColor;
    
    public Main()
	{
        this.InitializeComponent();
        this.usernameEntry.Completed += this.UsernameEntry_Completed;
        this.usernameEntry.Loaded += this.UsernameEntry_Loaded;
        this.actionButton.Clicked += this.ActionButton_Clicked;
        this.togglePunchTypeButton.Released += this.TogglePunchTypeButton_Released;
        this.passwordEntry.Completed += this.PasswordEntry_Completed;
        base.SetViewModelBinding<MainViewModel>();
        this._viewModel = (base.ViewModel as MainViewModel)!;
        this.SetToasts();
    }

    private async void ActionButton_Clicked(object? sender, EventArgs e) => await this.ProcessCredentials();

    private async ValueTask ProcessCredentials()
    {
        this.usernameEntry.IsEnabled = false;
        await this._viewModel.PunchCommand.ExecuteAsync(null);

        string name = this._viewModel.UserFullNameOr;
        if (string.IsNullOrWhiteSpace(name)) name = this._viewModel.UserName;
        
        this._viewModel.Clear();
        this.usernameEntry.IsEnabled = true;
        this.usernameEntry.Focus();

        if (this._viewModel.IsPunchHandled) return;

        await this.DisplayLoginAttemptMessage(this._viewModel.IsLoginSuccess, name, this._viewModel.DateTime, this._viewModel.CurrentPunchStatus, this._viewModel.HttpStatusCode,
            this._viewModel.IsMissingPunch, this._viewModel.MissingPunchDate);
        //this.usernameEntry.Focus();
    }

    private Task DisplayLoginAttemptMessage(bool success, string name, DateTime dateTime, PunchStatusDto? punchStatus, HttpStatusCode statusCode, bool isMissingPunch = false, DateOnly? missingPunchDate = null)
    {
        string message;
        string? secondaryMessage = null;
        ToastSeverityLevel toastSeverityLevel = ToastSeverityLevel.None;
        if (string.IsNullOrWhiteSpace(name))
        {
            name = CommonValues.InvalidUser;
            success = false;
        }

        if (success && (((int)statusCode) >= 200 && ((int)statusCode) < 300))
        {
            message = $"{this.Salutation} {name}\n{punchStatus} at {dateTime:t}";
            //punch out blue
            toastSeverityLevel = punchStatus == PunchStatusDto.In ? ToastSeverityLevel.Success : ToastSeverityLevel.Info;
            if (isMissingPunch)
                secondaryMessage = $"Missing Punch! {(missingPunchDate.HasValue ? missingPunchDate.Value.ToString("MM/dd/yyyy") : "Check History")}!";
        }
        else
        {
            toastSeverityLevel = ToastSeverityLevel.Error;
            message = $"{CommonValues.Error} {name}\n{statusCode}";
        }
        
        return this.ShowToast(message, toastSeverityLevel, secondaryMessage: secondaryMessage);
    }

    private async Task ShowToast(string message, ToastSeverityLevel severity, uint duration = 1000, string? secondaryMessage = null)
    {
        double topEdgeBuffer = this.titleLabel.GetAbsolutePosition().Y + (Math.Max(this.titleLabel.HeightRequest, this.titleLabel.Height));
        int negateNumber = -1;
        // because we are moving and changing the size, the animation moves above the y position that is being used to move.
        // in other words, the size expands both up and down, causing the y coordinate used to track location to not be accurate at the end
        // the magic number is approximately how much the animation grew above the y coordinate, using a rel-scale factor of 2.
        // this is all dependent on the font size and padding
        int magicNumber = string.IsNullOrWhiteSpace(secondaryMessage) ? 80 : 120;
        Easing easing = Easing.CubicInOut;
        KeyValuePair<DoubleLabel, bool> next = this._toasts.FirstOrDefault(t => !t.Value);
        int available = this._toasts.Count(t => !t.Value);
        double yOrigin = next.Key.GetAbsolutePosition().Y;
        double scale = 2;
        double yTarget = topEdgeBuffer - yOrigin + magicNumber;
        Color? color;
        Color entryBgColor = Colors.Transparent;

        yTarget = Math.Min(yTarget, yTarget * negateNumber);

        // if available is less than 2, that means we are working with the last one available, so 0 are actually available
        if (available < 2)
        {
            this.passwordEntry.IsEnabled = this.usernameEntry.IsEnabled = this.actionButton.IsEnabled = false;
            entryBgColor = this.usernameEntry.BackgroundColor;
            this.usernameEntry.BackgroundColor = Colors.Red;
            await TextToSpeech.Default.SpeakAsync(CommonValues.PleaseWait);
        }

        this._toastsColor.TryGetValue(severity, out color);

        color ??= Colors.White;

        next.Key.PrimaryLabel.TextColor = color;
        next.Key.PrimaryLabel.Text = message;
        next.Key.IsVisible = true;

        if (!string.IsNullOrWhiteSpace(secondaryMessage))
        {
            next.Key.SecondaryLabel.Text = secondaryMessage;
            next.Key.SecondaryLabel.TextColor = Colors.White;
            next.Key.SecondaryLabel.BackgroundColor = Colors.Red;
            next.Key.SecondaryLabel.Padding = new Thickness(20, 5);
            next.Key.SecondaryLabel.IsVisible = true;
            await next.Key.SecondaryLabel.BlinkBackground(Colors.Red);
        }
        else
        {
            next.Key.SecondaryLabel.IsVisible = false;
        }

        if (!this._toasts.TryUpdate(next.Key, true, false))
            return;

        // move visual element to top with message
        await Task.WhenAll(
            next.Key.FadeTo(1, duration, easing),
            next.Key.RelScaleTo(scale, duration, easing),
            next.Key.TranslateTo(0, yTarget, duration, easing)
        );

        duration = 3000;
        easing = Easing.Linear;
        //// hide any previous messages still on screen
        //// since the delay on screen is rather long, we do not want our messages to overlap
        //// we want ur current message visible when it takes a pause and begins to fade out
        //// even though the previous ones should be moving out of the way, we cannot guarantee it
        //// so best to hide it instantly. the transition to reset will finish, but will no longer be visible
        //foreach(var previous in this._toasts.Where(t => t.Value))
        //{
        //    previous.Key.IsVisible = false;
        //}

        // fade out the visual element containing our message
        //// hide any previous messages still on screen
        //// since the delay on screen is rather long, we do not want our messages to overlap
        //// we want ur current message visible when it takes a pause and begins to fade out
        //// even though the previous ones should be moving out of the way, we cannot guarantee it
        //// so best to hide it instantly. the transition to reset will finish, but will no longer be visible
        //foreach(var previous in this._toasts.Where(t => t.Value))
        //{
        //    previous.Key.IsVisible = false;
        //}

        // fade out the visual element containing our message
        await Task.WhenAll(
            next.Key.FadeTo(0, duration*2, easing),
            next.Key.RelScaleTo(scale*-1, duration*2, easing),
            next.Key.FadeTo(0, duration*2, easing),
            next.Key.RelScaleTo(scale*-1, duration*2, easing)
        );
        // reset back to original position
        await next.Key.TranslateTo(0, yOrigin, duration, easing);
        //// reset the items before we change the state in the dictionary for the current
        //foreach (var previous in this._toasts.Where(t => t.Value))
        //{
        //    previous.Key.IsVisible = true;
        //}
        //// reset the items before we change the state in the dictionary for the current
        //foreach (var previous in this._toasts.Where(t => t.Value))
        //{
        //    previous.Key.IsVisible = true;
        //}
        next.Key.IsVisible = true;
        this._toasts.TryUpdate(next.Key, false, true);

        next.Key.SecondaryLabel.IsVisible = false;
        next.Key.SecondaryLabel.BlinkBackgroundCancel();

        // we have finished processing the last one, and we were at the < 2 mark. so re-enable punch-in ability
        if (available < 2)
        {
            this.passwordEntry.IsEnabled = this.usernameEntry.IsEnabled = this.actionButton.IsEnabled = true;
            this.usernameEntry.Focus();
            this.usernameEntry.BackgroundColor = entryBgColor;
            await TextToSpeech.Default.SpeakAsync(CommonValues.YouMayRezoom);
        }
    }

    private void SetToasts()
    {
        int concurrency = Environment.ProcessorCount * 2;
        this._toasts = new ConcurrentDictionary<DoubleLabel, bool>(concurrency,
            this.toaster.GetVisualTreeDescendants().Where(x => x.GetType() == typeof(DoubleLabel)).ToDictionary(d => (DoubleLabel)d, d => false)
            , default);
        this._toastsColor = new ConcurrentDictionary<ToastSeverityLevel, Color>(concurrency,
            new System.Collections.Generic.KeyValuePair<ToastSeverityLevel, Color>[] {
                new System.Collections.Generic.KeyValuePair<ToastSeverityLevel, Color>(ToastSeverityLevel.None, Color.FromRgba("#e2e3e5")),
                new System.Collections.Generic.KeyValuePair<ToastSeverityLevel, Color>(ToastSeverityLevel.Info, Color.FromRgba("#99cbff")),
                new System.Collections.Generic.KeyValuePair<ToastSeverityLevel, Color>( ToastSeverityLevel.Warning, Color.FromRgba("#ffff8f")),
                new System.Collections.Generic.KeyValuePair<ToastSeverityLevel, Color>( ToastSeverityLevel.Error, Color.FromRgba("#f35e6e")),
                new System.Collections.Generic.KeyValuePair<ToastSeverityLevel, Color>(ToastSeverityLevel.Success, Color.FromArgb("#65f588"))
        }, default);
    }

    private async void UsernameEntry_Completed(object? sender, EventArgs e)
    {
        await this.ProcessCredentials();
    }

    private async void PasswordEntry_Completed(object? sender, EventArgs e)
    {
        await this.ProcessCredentials();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        this._viewModel.IsShown = true;
        this.usernameEntry.IsEnabled = true;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        this._viewModel.IsShown = false;
    }

    private void UsernameEntry_Loaded(object? sender, EventArgs e)
    {
        this.usernameEntry.Focus();
    }

    private void TogglePunchTypeButton_Released(object? sender, EventArgs e)
    {
        this.usernameEntry.Focus();
    }

    public string Salutation
    {
        get => Main.GetSalutation();
        private set { }
    }


    private static string GetSalutation()
    {
        int now = DateTime.Now.TimeOfDay.Hours;

        switch (now)
        {
            case < 12:
                return "Good Morning";
            case < 17:
                return "Good Afternoon";
            default:
                return "Good Evening";
        }
    }
}

public enum ToastSeverityLevel
{
    None,
    Info,
    Warning,
    Error,
    Success
}