using TimeClock.Maui.ViewModels.AdminSection;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views.AdminSection;

public partial class ManagePunches : BaseContentView
{
    public static readonly BindableProperty UserFirstNameProperty = BindableProperty.Create(nameof(UserFirstName), typeof(string), typeof(ManagePunches),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (ManagePunches)bindable;
            if (control?.ViewModel is not null && newValue is not null)
                control.ViewModel.UserFirstName = (string)newValue;
        });
    public static readonly BindableProperty UserLastNameProperty = BindableProperty.Create(nameof(UserName), typeof(string), typeof(ManagePunches),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (ManagePunches)bindable;
            if (control?.ViewModel is not null && newValue is not null)
                control.ViewModel.UserLastName = (string)newValue;
        });
    public static readonly BindableProperty UserEmployeeNumberProperty = BindableProperty.Create(nameof(ManagePunches.UserEmployeeNumber), typeof(string), typeof(ManagePunches),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (ManagePunches)bindable;
            if (control?.ViewModel is not null && newValue is not null)
                control.ViewModel.UserEmployeeNumber = (string)newValue;
        });
    public static readonly BindableProperty UserNameProperty = BindableProperty.Create(nameof(ManagePunches.UserName), typeof(string), typeof(ManagePunches),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (ManagePunches)bindable;
            if (control?.ViewModel is not null && newValue is not null)
                control.ViewModel.UserName = (string)newValue;
        });
    public ManagePunches()
	{
        this.InitializeComponent();
		base.SetViewModelBinding<ManagePunchesViewModel>();
    }
    public string UserFirstName
    {
        get => (string)base.GetValue(ManagePunches.UserFirstNameProperty);
        set => base.SetValue(ManagePunches.UserFirstNameProperty, value);
    }
    public string UserLastName
    {
        get => (string)base.GetValue(ManagePunches.UserLastNameProperty);
        set => base.SetValue(ManagePunches.UserLastNameProperty, value);
    }
    public string UserEmployeeNumber
    {
        get => (string)base.GetValue(ManagePunches.UserEmployeeNumberProperty);
        set => base.SetValue(ManagePunches.UserEmployeeNumberProperty, value);
    }
    public string UserName
    {
        get => (string)base.GetValue(ManagePunches.UserNameProperty);
        set => base.SetValue(ManagePunches.UserNameProperty, value);
    }
}