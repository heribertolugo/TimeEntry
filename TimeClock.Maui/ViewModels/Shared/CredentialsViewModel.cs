using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeClock.Maui.ViewModels.Shared;
public sealed partial class CredentialsViewModel : ObservableObject //: ViewModelBase
{
    [ObservableProperty]
    private string? title;
    [ObservableProperty]
    private string? message;
    [ObservableProperty]
    private string? userName;
    [ObservableProperty]
    private string? password;
    [ObservableProperty]
    private int? employeeId;
    [ObservableProperty]
    private bool showEmployeeId = false;
    //protected override Task Refresh() => throw new NotImplementedException();
}
