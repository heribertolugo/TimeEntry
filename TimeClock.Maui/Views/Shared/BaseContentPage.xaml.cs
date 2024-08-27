using TimeClock.Maui.Helpers;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.Views.Shared;

/// <summary>
/// Using this as a base requires the viewmodel to implement IViewModel
/// </summary>
public partial class BaseContentPage : ContentPage
{
    public BaseContentPage()
    {
        this.InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        IViewModel? viewModel = base.BindingContext as IViewModel;
        if (viewModel != null)
            viewModel.RefreshCommand.Execute(this);
    }

    protected virtual IViewModel? ViewModel { get; set; }
    protected void SetViewModelBinding<T>() where T : class, IViewModel
    {
        this.ViewModel = ServiceHelper.GetService<T>() ?? throw new ApplicationException(string.Format(CommonValues.FatalNotFound, typeof(T).Name));
        base.BindingContext = this.ViewModel;
    }
}