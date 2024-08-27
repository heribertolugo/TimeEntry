using TimeClock.Maui.Helpers;
using TimeClock.Maui.ViewModels.HistorySection;
using TimeClock.Maui.Views.Shared;

namespace TimeClock.Maui.Views.HistorySection;

public partial class EquipmentHistory : BaseContentView
{
    public static readonly BindableProperty GridColumnSpacingProperty = BindableProperty.Create(nameof(GridColumnSpacing), typeof(double), typeof(EquipmentHistory));
    public static readonly BindableProperty BorderPaddingProperty = BindableProperty.Create(nameof(BorderPadding), typeof(Thickness), typeof(EquipmentHistory));

    private Thickness _borderPadding;
    private double _gridColumnSpacing;

    public EquipmentHistory():base()
    {
        base.SetViewModelBinding<EquipmentHistoryViewModel>();
        InitializeComponent();
        this._borderPadding = base.IsNotMobileDevice ? new Thickness(40, 0) : new Thickness(10, 0);
        this._gridColumnSpacing = base.IsNotMobileDevice ? 40 : 20;
    }

    public Thickness BorderPadding
    {
        get => this._borderPadding;
        set => SetValue(BorderPaddingProperty, this._borderPadding);
    }
    public double GridColumnSpacing
    {
        get => this._gridColumnSpacing;
        set => SetValue(GridColumnSpacingProperty, this._gridColumnSpacing);
    }
}