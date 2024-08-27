namespace TimeClock.Maui.Controls;

public partial class ToggleSwitch : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(ToggleSwitch.Text), typeof(string), typeof(ToggleSwitch), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (string)newValue;

        if (control.IsChecked && control.OnText != null)
            control.Label.Text = control.OnText;
        else if (!control.IsChecked && control.OffText != null)
            control.Label.Text = control.OffText;
        else
            control.Label.Text = value ?? string.Empty;
    });
    public static readonly BindableProperty OnTextProperty = BindableProperty.Create(nameof(ToggleSwitch.OnText), typeof(string), typeof(ToggleSwitch), null, propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (string)newValue;

        if (control.IsChecked)
            control.Label.Text = value;
        else if (!control.IsChecked && control.OffText != null)
            control.Label.Text = control.OffText;
        else
            control.Label.Text = control.Text ?? string.Empty;
    });
    public static readonly BindableProperty OffTextProperty = BindableProperty.Create(nameof(ToggleSwitch.OffText), typeof(string), typeof(ToggleSwitch), null, propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (string)newValue;

        if (!control.IsChecked)
            control.Label.Text = value;
        else if (control.IsChecked && control.OnText != null)
            control.Label.Text = control.OnText;
        else
            control.Label.Text = control.Text ?? string.Empty;
    });
    public static readonly BindableProperty OnTextColorProperty = BindableProperty.Create(nameof(ToggleSwitch.OnTextColor), typeof(Color), typeof(ToggleSwitch), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (Color)newValue;

        if (control.IsChecked)
            control.TextColor = value;
        else if (!control.IsChecked && control.OffTextColor != null)
            control.TextColor = control.OffTextColor;
    });
    public static readonly BindableProperty OffTextColorProperty = BindableProperty.Create(nameof(ToggleSwitch.OffTextColor), typeof(Color), typeof(ToggleSwitch), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (Color)newValue;

        if (!control.IsChecked)
            control.TextColor = value;
        else if (control.IsChecked && control.OnTextColor != null)
            control.TextColor = control.OnTextColor;
    });
    public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(ToggleSwitch.IsChecked), typeof(bool), typeof(ToggleSwitch), defaultValue:false, propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (bool)newValue;

        ToggleSwitch.AnimateSwitch(control, value);
    });
    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(ToggleSwitch.TextColor), typeof(Color), typeof(ToggleSwitch), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (Color)newValue;
        control.Label.TextColor = value;
    });
    public static readonly BindableProperty OnThumbColorProperty = BindableProperty.Create(nameof(ToggleSwitch.OnThumbColor), typeof(Color), typeof(ToggleSwitch), Colors.Green);
    public static readonly BindableProperty OffThumbColorProperty = BindableProperty.Create(nameof(ToggleSwitch.OffThumbColor), typeof(Color), typeof(ToggleSwitch), Colors.Red);
    public static readonly BindableProperty OnTrackColorProperty = BindableProperty.Create(nameof(ToggleSwitch.OnTrackColor), typeof(Color), typeof(ToggleSwitch), Colors.Grey);
    public static readonly BindableProperty OffTrackColorProperty = BindableProperty.Create(nameof(ToggleSwitch.OffTrackColor), typeof(Color), typeof(ToggleSwitch), Colors.Grey);
    public static readonly new BindableProperty HeightRequestProperty = BindableProperty.Create(nameof(ToggleSwitch.HeightRequest), typeof(double), typeof(ToggleSwitch), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (ToggleSwitch)bindable;
        var value = (double)newValue;

        if (control.Thumb is null || control.SwitchLayout is null) return;
        var bounds = control.SwitchLayout.GetLayoutBounds(control.Thumb);

        control.SwitchLayout.SetLayoutFlags(control.Thumb, Microsoft.Maui.Layouts.AbsoluteLayoutFlags.PositionProportional | Microsoft.Maui.Layouts.AbsoluteLayoutFlags.HeightProportional);
        control.SwitchLayout.SetLayoutBounds(control.Thumb, new Rect(
            bounds.X,
            bounds.Y,
            value,
            bounds.Height));
        // hacky way to fix bug when height set too big, the thumb looses its coordinates
        control.IsChecked = !control.IsChecked;
        control.IsChecked = !control.IsChecked;
    });
    #endregion Bindable Properties

    public ToggleSwitch()
	{
		this.InitializeComponent();
        this.IsChecked = !this.IsChecked;
        this.IsChecked = !this.IsChecked;
	}

    #region Public Properties
    public string? Text
    {
        get => (string)base.GetValue(TextProperty);
        private set => base.SetValue(TextProperty, value);
    }
    public string? OnText
    {
        get => (string)base.GetValue(OnTextProperty);
        set => base.SetValue(OnTextProperty, value);
    }
    public string? OffText
    {
        get => (string)base.GetValue(OffTextProperty);
        set => base.SetValue(OffTextProperty, value);
    }
    public Color? OnTextColor
    {
        get => (Color)base.GetValue(OnTextColorProperty);
        set => base.SetValue(OnTextColorProperty, value);
    }
    public Color? OffTextColor
    {
        get => (Color)base.GetValue(OffTextColorProperty);
        set => base.SetValue(OffTextColorProperty, value);
    }
    public bool IsChecked
    {
        get => (bool)base.GetValue(IsCheckedProperty);
        set => base.SetValue(IsCheckedProperty, value);
    }
    public Color TextColor
    {
        get => (Color)base.GetValue(TextColorProperty);
        set => base.SetValue(TextColorProperty, value);
    }
    public Color OnThumbColor
    {
        get => (Color)base.GetValue(OnThumbColorProperty);
        set => base.SetValue(OnThumbColorProperty, value);
    }
    public Color OffThumbColor
    {
        get => (Color)base.GetValue(OffThumbColorProperty);
        set => base.SetValue(OffThumbColorProperty, value);
    }
    public Color OnTrackColor
    {
        get => (Color)base.GetValue(OnTrackColorProperty);
        set => base.SetValue(OnTrackColorProperty, value);
    }
    public Color OffTrackColor
    {
        get => (Color)base.GetValue(OffTrackColorProperty);
        set => base.SetValue(OffTrackColorProperty, value);
    }
    public new double HeightRequest
    {
        get => (double)base.GetValue(HeightRequestProperty);
        set
        {
            base.HeightRequest = value;
            base.SetValue(HeightRequestProperty, value);
        }
    }
    #endregion Public Properties


    private static void AnimateSwitch(ToggleSwitch control, bool isChecked)
    {
        uint rate = 16u;
        Rect bounds = control.SwitchLayout.GetLayoutBounds(control.Thumb);
        ColorTransition thumbTransition = new(
            (isChecked ? control.OffThumbColor : control.OnThumbColor),
            (isChecked ? control.OnThumbColor : control.OffThumbColor),
            rate);
        ColorTransition trackTransition = new(
            (isChecked ? control.OffTrackColor : control.OnTrackColor),
            (isChecked ? control.OnTrackColor : control.OffTrackColor),
            rate);
        Animation parentAnimation = new();

        if (control.OnTextColor != null || control.OnTextColor != null)
        {
            ColorTransition textColorTransition = new(
                (isChecked ? control.OffTextColor : control.OnTextColor) ?? control.TextColor,
                (isChecked ? control.OnTextColor : control.OffTextColor) ?? control.TextColor,
                rate);
            parentAnimation.Add(0, 1, new Animation(callback: v => control.Label.TextColor = textColorTransition.GetNext(v), start: 0, end: rate));
        }

        parentAnimation.Add(0, 1, new(callback: v => control.Thumb.Fill = thumbTransition.GetNext(v), start: 0, end: rate));
        parentAnimation.Add(0, 1, new(callback: v => control.Track.Fill = trackTransition.GetNext(v), start: 0, end: rate));
        parentAnimation.Add(0, 1, new(callback: v => control.SwitchLayout.SetLayoutBounds(control.Thumb, new(
            v,
            bounds.Y,
            bounds.Width,
            bounds.Height)), isChecked ? 0 : 1, isChecked ? 1 : 0));

        parentAnimation.Commit(control, "toggleSwitchAnimation", rate, 250, Easing.CubicInOut, (v, c) => control.Text = isChecked ? (control.OnText??control.Text??string.Empty) : (control.OffText ?? control.Text ?? string.Empty));
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        this.IsChecked = !this.IsChecked;
    }
}


internal class ColorTransition
{
    private float _redRate, _greenRate, _blueRate;
    private byte _oldColorR, _oldColorG, _oldColorB;
    public ColorTransition(Color from, Color to, uint rate)
    {
        this.From = this.Current = from;
        this.To = to;
        this.Rate = rate;
        this.SetRatesOfChange(from,to,rate);
    }

    public Color GetNext(double v)
    {
        this.Current = Color.FromRgb(
            (byte)Math.Min(Math.Max(this._oldColorR + (v * this._redRate), 0), 255),
            (byte)Math.Min(Math.Max(this._oldColorG + (v * this._greenRate), 0), 255),
            (byte)Math.Min(Math.Max(this._oldColorB + (v * this._blueRate), 0), 255));
        return this.Current;
    }

    private void SetRatesOfChange(Color from, Color to, uint rate)
    {
        byte newColorR, newColorG, newColorB;
        to  .ToRgb(out newColorR, out newColorG, out newColorB);
        from.ToRgb(out this._oldColorR, out this._oldColorG, out this._oldColorB);

        this._redRate   = this._oldColorR == newColorR ? newColorR / rate : (newColorR - this._oldColorR) / rate;
        this._greenRate = this._oldColorG == newColorG ? newColorG / rate : (newColorG - this._oldColorG) / rate;
        this._blueRate  = this._oldColorB == newColorB ? newColorB / rate : (newColorB - this._oldColorB) / rate;
    }

    public Color From { get; init; }
    public Color To { get; init; }
    public uint Rate { get; init; }
    public Color Current { get; set; }
}