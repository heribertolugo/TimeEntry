using TimeClock.Maui.ViewModels.AdminSection;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views.AdminSection;

public partial class ManageEmployees : BaseContentView
{
	public ManageEmployees():base()
	{
		base.SetViewModelBinding<ManageEmployeesViewModel>();
		this.InitializeComponent();
    }
}