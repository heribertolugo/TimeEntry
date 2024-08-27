using CommunityToolkit.Maui.Views;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels.AdminSection;

namespace TimeClock.Maui.Views.AdminSection;

public partial class EditEntryPopup : Popup
{
	public EditEntryPopup()
	{
        this.InitializeComponent();
    }

    public async void OnSave_Clicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(CommonValues.ApiCancellationTokenLimit);
        EditEntryViewModel? viewModel = this.BindingContext as EditEntryViewModel;
        if (viewModel is null)
            throw new ArgumentNullException("Binding context was not of expected type");
        if (viewModel.Item is null)
            throw new ArgumentNullException("Binding context item was not found");
        await this.CloseAsync(true, cts.Token);
        await viewModel.Item.SaveEditingCommand.ExecuteAsync(null);
    }

    public async void OnCancel_Clicked(object? sender, EventArgs e)
    {
        var cts = new CancellationTokenSource(CommonValues.ApiCancellationTokenLimit);
        EditEntryViewModel? viewModel = this.BindingContext as EditEntryViewModel;
        if (viewModel is null)
            throw new ArgumentNullException("Binding context was not of expected type");
        
        await this.CloseAsync(true,  cts.Token);
        
        if (viewModel.Item is not null)
            await viewModel.Item.CancelEditingCommand.ExecuteAsync(null);
    }

    protected override Task OnDismissedByTappingOutsideOfPopup(CancellationToken token = default)
    {
        EditEntryViewModel? viewModel = this.BindingContext as EditEntryViewModel;
        if (viewModel is null)
            throw new ArgumentNullException("Binding context was not of expected type");
        if (viewModel.Item is null)
            return base.OnDismissedByTappingOutsideOfPopup(token); 
        viewModel.Item.CancelEditingCommand.ExecuteAsync(null);
        return base.OnDismissedByTappingOutsideOfPopup(token);
    }
}