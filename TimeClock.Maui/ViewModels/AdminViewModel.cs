using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using TimeClock.Maui.Controls;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.ViewModels;

public sealed class AdminViewModel : ViewModelBase
{
    private bool _needsDatePicker;
    private static readonly string[] NeedDatePickerTabNames = [ nameof(AdminViewTabProperties.Summary) ];
    private Timer _timer;
    public AdminViewModel() : base()
    {
        this.TabChangedCommand = new AsyncRelayCommand<TabControlItem?>(this.HandleTabChanged);
        //_timer = new Timer(
        //    new TimerCallback((that) => 
        //    {
        //        ((AdminViewModel)that).TotalOpenRequests = DateTime.Now.Second;
        //    }),
        //    this,
        //    TimeSpan.Zero,
        //    TimeSpan.FromSeconds(10)
        //    );
    }

    public bool NeedsDatePicker
    {
        get => this._needsDatePicker;
        set => base.SetProperty(ref this._needsDatePicker, value);
    }
    public ICommand TabChangedCommand { get; private set; }
    private bool EmployeesTabHasShown { get; set; } = false;
    private Task HandleTabChanged(TabControlItem? tab)
    {
        if (tab != null)
        {
            this.NeedsDatePicker = AdminViewModel.NeedDatePickerTabNames.Contains(tab.Name);
            //(tab.Content.BindingContext as IViewModel)?.ImportUser(this);
            // handles weird glitch where
            // the Margin 60 added (to prevent CollectionView from growing past bottom of screen) to offset parent padding 
            // now causes the CollectionView to decrease 60 in height, when tab is changed.
            // so we will remove that Margin 60 so that the CollectionView height stays consistent
#warning this might not be needed anymore
            if (!string.IsNullOrWhiteSpace(tab.Name) && tab.Name.ToLower().Equals(AdminViewTabProperties.EditEntry))
            {
                if (this.EmployeesTabHasShown)
                    (tab.Content.FindByName("PrimaryCollectionView") as CollectionView)!.Margin = new Thickness(0);
                this.EmployeesTabHasShown = true;
            }
        }

        return Task.CompletedTask;
    }

    protected override Task Refresh()
    {
        this.TotalOpenRequests = DateTime.Now.Second;
        return Task.CompletedTask;
    }
    public override DateTime FilterDate 
    { 
        get => base.FilterDate; 
        set => base.FilterDate = value; 
    }
    public override Guid UserId 
    { 
        get => base.UserId; 
        set => base.UserId = value; 
    }
    private int? _totalOpenRequests;
    public int? TotalOpenRequests
    {
        get => this._totalOpenRequests;
        set => base.SetProperty(ref this._totalOpenRequests, value);
    }
}

public sealed class AdminViewTabProperties
{
    public readonly static AdminViewTabProperties Summary = new("Summary");
    public readonly static AdminViewTabProperties EditEntry = new("Edit Punches");
    public readonly static AdminViewTabProperties Employees = new("Employees");
    public readonly static AdminViewTabProperties PunchRequests = new("Requests");
    private AdminViewTabProperties(string name)
    {
        this.Name = name;
    }

    public string Name { get; private set; }

    public static implicit operator string(AdminViewTabProperties a) => a.Name;
}
