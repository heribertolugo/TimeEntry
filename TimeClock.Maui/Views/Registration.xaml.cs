using TimeClock.Maui.Helpers;
using TimeClock.Maui.ViewModels;

namespace TimeClock.Maui.Views;

public partial class Registration : ContentPage
{
	public Registration()
	{
		this.InitializeComponent();
	}

    protected override async void OnAppearing()
	{
		base.OnAppearing();
		var vm = ServiceHelper.GetService<RegistrationViewModel>() 
			?? throw new ApplicationException(string.Format(CommonValues.FatalNotFound, typeof(RegistrationViewModel).Name));
        base.BindingContext = vm;
		vm.CredentialsContainer = this.credentialsWrapper;
		vm.MessagesLabel = this.messageDummy;
        await vm.BeginRegistration();
	}
}