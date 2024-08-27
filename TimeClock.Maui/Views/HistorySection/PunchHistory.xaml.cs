using TimeClock.Maui.ViewModels.HistorySection;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views.HistorySection;

public partial class PunchHistory : BaseContentView
{
    public static readonly BindableProperty GridColumnSpacingProperty = BindableProperty.Create(nameof(GridColumnSpacing), typeof(double), typeof(PunchHistory));
    public static readonly BindableProperty BorderPaddingProperty = BindableProperty.Create(nameof(BorderPadding), typeof(Thickness), typeof(PunchHistory));

    private Thickness _borderPadding;
    private readonly double _gridColumnSpacing;

    public PunchHistory():base()
	{
        base.SetViewModelBinding<PunchHistoryViewModel>();
        this.InitializeComponent();
        this._borderPadding = this.IsNotMobileDevice ? new Thickness(40, 0) : new Thickness(10, 0);
        this._gridColumnSpacing = this.IsNotMobileDevice ? 40 : 20;
    }

    public Thickness BorderPadding
    {
        get => this._borderPadding;
        set => base.SetValue(BorderPaddingProperty, this._borderPadding);
    }
    public double GridColumnSpacing
    {
        get => this._gridColumnSpacing;
        set => base.SetValue(GridColumnSpacingProperty, this._gridColumnSpacing);
    }
    public override Guid UserId
    {
        get => base.UserId;
        set
        {
            base.UserId = value;
            if (this.BindingContext is PunchHistoryViewModel vm)
                vm.UserId = value;
        }
    }
}