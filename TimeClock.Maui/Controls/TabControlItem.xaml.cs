namespace TimeClock.Maui.Controls;

/// <summary>
/// TabControlItem 
/// </summary>
public partial class TabControlItem : ContentView
{
    #region Public Bindable Properties
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(TabControlItem),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (TabControlItem)bindable;
            if (control.Name == null)
                control.Name = newValue as string;
        });
    public static readonly BindableProperty NameProperty = BindableProperty.Create(nameof(Name), typeof(string), typeof(TabControlItem),
        propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (TabControlItem)bindable;
            if (control.Title == null)
                control.Title = newValue as string;
        });
    public static readonly BindableProperty BadgeValueProperty = BindableProperty.Create(nameof(BadgeValue), typeof(int?), typeof(TabControlItem),
        defaultValue: null, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (TabControlItem)bindable;
            control.BadgeValue = newValue as int?;
            control.IsTabBadgeVisible = control.BadgeValue is not null;
        });
    public static readonly BindableProperty IsTabBadgeVisibleProperty = BindableProperty.Create(nameof(IsTabBadgeVisible), typeof(bool), typeof(TabControlItem),
        defaultValue: false, propertyChanged: (bindable, oldValue, newValue) =>
        {
            var control = (TabControlItem)bindable;
            control.IsTabBadgeVisible = control.BadgeValue is not null;
        });
    #endregion Public Bindable Properties

    #region Constructors
    public TabControlItem()
    {
    }
    public TabControlItem(string title) : this(title, title, null) { }
    public TabControlItem(string title, string name) : this(title, name, null) { }
    public TabControlItem(string title, string name, int? badgeValue) : this()
    {
        this.Title = title;
        this.Name = name;
        this.BadgeValue = badgeValue;
        this.InitializeComponent();
    }
    #endregion Constructors


    #region Public Properties
    public string? Title
    {
        get => (string)base.GetValue(TitleProperty);
        set
        {
            base.SetValue(TitleProperty, value);
            if (this.Name == null)
                this.Name = value;
        }
    }

    public string? Name
    {
        get => (string)base.GetValue(NameProperty);
        set
        {            
            base.SetValue(NameProperty, value);
            if (this.Title == null)
                this.Title= value;
        }
    }

    public int? BadgeValue
    {
        get => (int?)base.GetValue(BadgeValueProperty);
        set
        {
            base.SetValue(BadgeValueProperty, value);
            //this.IsTabBadgeVisible = value is not null;
        }
    }

    public bool IsTabBadgeVisible
    {
        get => (bool)base.GetValue(IsTabBadgeVisibleProperty);
        private set => base.SetValue(IsTabBadgeVisibleProperty, value);
    }
    #endregion Public Properties
}