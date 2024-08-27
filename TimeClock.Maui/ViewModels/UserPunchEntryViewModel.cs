using CommunityToolkit.Mvvm.ComponentModel;

namespace TimeClock.Maui.ViewModels;

[QueryProperty(nameof(FirstName), nameof(FirstName))]
[QueryProperty(nameof(LastName), nameof(LastName))]
[QueryProperty(nameof(UserName), nameof(UserName))]
[QueryProperty(nameof(EmployeeNumber), nameof(EmployeeNumber))]
[QueryProperty(nameof(Punch), nameof(Punch))]
[QueryProperty(nameof(Action), nameof(Action))]
internal sealed class UserPunchEntryViewModel : ObservableObject
{
    private string _firstName;
    private string _lastName;
    private string _userName;
    private string _employeeNumber;
    private DateTime _punch;
    //private PunchType _punchType;
    private string _action;

    public UserPunchEntryViewModel() 
    {
        this._firstName = string.Empty;
        this._lastName = string.Empty;
        this._userName = string.Empty;
        this._employeeNumber = string.Empty;
        //this._punchType = PunchType.Barcode;// default(PunchType);
        this._punch = DateTime.MinValue;
        this._action = string.Empty;
    }

    public string FirstName { get => this._firstName; set => base.SetProperty(ref this._firstName, value); }
    public string LastName { get => this._lastName; set => base.SetProperty(ref this._lastName, value); }
    public string UserName { get => this._userName; set => base.SetProperty(ref this._userName, value); }
    public string EmployeeNumber { get => this._employeeNumber; set => base.SetProperty(ref this._employeeNumber, value); }
    //public PunchType PunchType { get => this._punchType; set => SetProperty(ref this._punchType, value); }
    public DateTime Punch { get => this._punch; set => base.SetProperty(ref this._punch, value); }
    public string Action { get => this._action; set => base.SetProperty(ref this._action, value); }
    public string Salutation
    {
        get => UserPunchEntryViewModel.GetSalutation();
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
