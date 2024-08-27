using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views;

public partial class Configurations : BaseContentPage
{
	public Configurations()
	{
        this.InitializeComponent();
    }

    private void SelectedScreenTimeoutEntry_TextChanged(object sender, TextChangedEventArgs e)
    {
        Entry entry = (Entry)sender;
        if (!int.TryParse(e.NewTextValue, out _))
            entry.Text = e.OldTextValue;
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        string text;
        if (sender is not Entry entry)
            return;
        text = entry.Text;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Clipboard.Default.SetTextAsync(text);
        });
    }
}