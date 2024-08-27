namespace TimeClock.Maui.Views.HistorySection;
using CommunityToolkit.Maui.Views;
using System.Threading.Tasks;
using TimeClock.Maui.ViewModels.AdminSection;
using TimeClock.Maui.ViewModels.HistorySection;

public partial class EditPunch : Popup
{
	public EditPunch(EditPunchViewModel viewModel)
	{
		InitializeComponent();
    }

	public async void SaveButton_Clicked(object sender, EventArgs e)
    {
        CancellationTokenSource token = new CancellationTokenSource(CommonValues.ApiCancellationTokenLimit);
        await CloseAsync(this.BindingContext as EditPunchViewModel, token: token.Token);
    }
	public async void CancelButton_Clicked(object sender, EventArgs e)
	{
        CancellationTokenSource token = new CancellationTokenSource(CommonValues.ApiCancellationTokenLimit);
        await CloseAsync(null, token: token.Token);
    }
}