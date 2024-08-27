namespace TimeClock.Maui.Controls;

public partial class DoubleLabel : ContentView
{

    public static readonly BindableProperty PrimaryLabelProperty = BindableProperty.Create(nameof(PrimaryLabel), typeof(Label), typeof(DoubleLabel));
    public static readonly BindableProperty SecondaryLabelProperty = BindableProperty.Create(nameof(SecondaryLabel), typeof(Label), typeof(DoubleLabel));
    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(DoubleLabel),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (DoubleLabel)bindable;
            var value = (TextAlignment)newValue;

            control.PrimaryLabel.HorizontalTextAlignment = value;
            control.SecondaryLabel.HorizontalTextAlignment = value;
        });

    public DoubleLabel()
	{
		this.InitializeComponent();
	}

    public Label PrimaryLabel => this.primaryLabel;
    public Label SecondaryLabel => this.secondaryLabel;
    public TextAlignment HorizontalTextAlignment { get; set; }
}