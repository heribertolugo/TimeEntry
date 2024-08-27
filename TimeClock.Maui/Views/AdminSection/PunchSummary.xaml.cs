using TimeClock.Maui.ViewModels.AdminSection;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views.AdminSection;

public partial class PunchSummary : BaseContentView
{
    public PunchSummary() : base()
	{
		InitializeComponent();
        base.SetViewModelBinding<PunchSummaryViewModel>();       
    }
}