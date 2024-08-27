using TimeClock.Maui.ViewModels.AdminSection;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views.AdminSection;

public partial class ManageRequests : BaseContentView
{
    public ManageRequests()
	{
		this.InitializeComponent();
        base.SetViewModelBinding<ManageRequestsViewModel>();
    }
}