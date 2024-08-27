using CommunityToolkit.Maui.Views;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.Views.Shared;

public partial class Credentials : Popup
{
	public Credentials()
	{
        this.InitializeComponent();
        this.passwordEntry.Completed += this.PasswordEntry_Completed;
	}

    private void PasswordEntry_Completed(object? sender, EventArgs e)
    {
        this.submitButton.SendClicked();
    }


    public async void SaveButton_Clicked(object sender, EventArgs e)
    {
        CancellationTokenSource token = new(CommonValues.ApiCancellationTokenLimit);
        await base.CloseAsync(this.BindingContext as CredentialsViewModel, token: token.Token);
    }
    public async void CancelButton_Clicked(object sender, EventArgs e)
    {
        CancellationTokenSource token = new(CommonValues.ApiCancellationTokenLimit);
        await base.CloseAsync(null, token: token.Token);
    }
}