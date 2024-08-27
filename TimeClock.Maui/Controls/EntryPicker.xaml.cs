using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using System.Collections;
using System.Reflection;
using static Microsoft.Maui.Controls.VisualMarker;

namespace TimeClock.Maui.Controls;

public partial class EntryPicker : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(EntryPicker));
    public static readonly BindableProperty CloseOnSelectedProperty = BindableProperty.Create(nameof(CloseOnSelected), typeof(bool), typeof(EntryPicker), true);
    public static readonly BindableProperty OnSelectedCommandProperty = BindableProperty.Create(nameof(OnSelectedCommand), typeof(IAsyncRelayCommand<object?>), typeof(EntryPicker));
    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(EntryPicker), defaultValue: null, propertyChanged: (bindable, oldVal, newVal) => 
    {
        var control = (EntryPicker)bindable;
        control.ListView.ItemsSource = (IEnumerable)newVal;
    });
    public static readonly BindableProperty DisplayMemberProperty = BindableProperty.Create(nameof(DisplayMember), typeof(string), typeof(EntryPicker), defaultValue: "");
    public static readonly BindableProperty DropDownItemTemplateProperty = BindableProperty.Create(nameof(DropDownItemTemplate), typeof(DataTemplate), typeof(EntryPicker), defaultValue: null, propertyChanged: (bindable, oldVal, newVal) => 
    {
        var control = (EntryPicker)bindable;
        control.ListView.ItemTemplate = (DataTemplate)newVal;
    });
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(EntryPicker), propertyChanged: (bindable, oldValue, newValue) =>
    {
        var control = (EntryPicker)bindable;
        control.SelectedItem = newValue;
        if (control.CloseOnSelected)
            control.Popup.CloseAsync(control.SelectedItem);
    });
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(EntryPicker), propertyChanged: (bindable, oldValue, newValue) =>
    {
        //var control = (EntryPicker)bindable;
        //var value = (string)newValue;
        //control.Popup.Title = value;
    }); 
    public static new readonly BindableProperty VisualProperty = BindableProperty.Create(nameof(Visual), typeof(IVisual), typeof(EntryPicker), defaultValue: new DefaultVisual(), propertyChanged: (bindable, oldVal, newVal) => 
    {
        var control = (EntryPicker)bindable;
        var visual = (IVisual)newVal;
        control.ListView.Visual = visual;
        control.SearchBar.Visual = visual;
        control.Label.Visual = visual;
    }); 
    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(EntryPicker), defaultValue: "", propertyChanged: (bindable, oldVal, newVal) => 
    {
        var control = (EntryPicker)bindable;
        var value = (string)newVal;
        control.SearchBar.Placeholder = value;
        control.DefaultText = value;
    });
    public static readonly BindableProperty DropDownStyleProperty = BindableProperty.Create(nameof(DropDownStyle), typeof(Style), typeof(EntryPicker), defaultValue: null, propertyChanged: (bindable, oldVal, newVal) =>
    {
        var control = (EntryPicker)bindable;
        control.ListView.Style = (Style)newVal;
    }); 
    public static readonly BindableProperty SearchBarStyleProperty = BindableProperty.Create(nameof(SearchBarStyle), typeof(Style), typeof(EntryPicker), defaultValue: null, propertyChanged: (bindable, oldVal, newVal) =>
    {
        var control = (EntryPicker)bindable;
        control.SearchBar.Style = (Style)newVal;
    });
    #endregion Bindable Properties

    #region Private Members
    private Popup Popup { get; set; }
    private string? DefaultText { get; set; }
    private ListView ListView { get; set; }
    private SearchBar SearchBar { get; set; }
    #endregion Private Members

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EntryPicker()
    {
		this.InitializeComponent();
        this.Popup = new Popup()
        {
            Content = this.CreatePopupContent()
        };
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


    #region Public Properties
    public bool CloseOnSelected
    {
        get => (bool)base.GetValue(CloseOnSelectedProperty);
        set => base.SetValue(CloseOnSelectedProperty, value);
    }
    public string Text
    {
        get => (string)base.GetValue(TextProperty);
        set
        {
            if (this.DefaultText is null)
                this.DefaultText = value;
            base.SetValue(TextProperty, value);
        }
    }
    public string? Title
    {
        get => (string)base.GetValue(TitleProperty);
        set => base.SetValue(TitleProperty, value);
    }
    public string DisplayMember
    {
        get { return (string)base.GetValue(DisplayMemberProperty); }
        set { base.SetValue(DisplayMemberProperty, value); }
    }
    public Style DropDownStyle
    {
        get { return (Style)base.GetValue(DropDownStyleProperty); }
        set { base.SetValue(DropDownStyleProperty, value); }
    }
    public object? SelectedItem
    {
        get => (object)base.GetValue(SelectedItemProperty);
        set => base.SetValue(SelectedItemProperty, value);
    }
    public IEnumerable ItemsSource
    {
        get { return (IEnumerable)base.GetValue(ItemsSourceProperty); }
        set { base.SetValue(ItemsSourceProperty, value); }
    }
    public DataTemplate DropDownItemTemplate
    {
        get { return (DataTemplate)base.GetValue(DropDownItemTemplateProperty); }
        set { base.SetValue(DropDownItemTemplateProperty, value); }
    }
    public Style SearchBarStyle
    {
        get { return (Style)base.GetValue(SearchBarStyleProperty); }
        set { base.SetValue(SearchBarStyleProperty, value); }
    }
    public new IVisual Visual
    {
        get { return (IVisual)base.GetValue(VisualProperty); }
        set { base.SetValue(VisualProperty, value); }
    }
    public string Placeholder
    {
        get { return (string)base.GetValue(PlaceholderProperty); }
        set { base.SetValue(PlaceholderProperty, value); }
    }
    #endregion Public Properties

    #region Public Commands
    public IAsyncRelayCommand ClosePopupCommand { get; set; }
    public IAsyncRelayCommand<object?> OnSelectedCommand
    {
        get => (IAsyncRelayCommand<object?>)base.GetValue(OnSelectedCommandProperty);
        set => base.SetValue(OnSelectedCommandProperty, value);
    }
    #endregion Public Commands

    #region Private Commands
    private Task ClosePopup()
    {
        return this.Popup?.CloseAsync() ?? Task.CompletedTask;
    }
    #endregion Private Commands

    #region Event Handlers
    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        var page = App.Current?.MainPage;

        if (page is null) return;

        this.Popup.Color = Colors.Black;
        this.Popup.CanBeDismissedByTappingOutsideOfPopup = false;

        await page.ShowPopupAsync(this.Popup);
    }
    private void ListView_ItemSelected(object? sender, SelectedItemChangedEventArgs e)
    {
        this.SelectedItem = e.SelectedItem;
        this.Text = ((!string.IsNullOrEmpty(this.DisplayMember) && e.SelectedItem is not null)
                    ? e.SelectedItem?.GetType().GetProperty(this.DisplayMember)?.GetValue(e.SelectedItem, null)?.ToString()
                    : e.SelectedItem?.ToString()) ?? this.DefaultText ?? string.Empty;

        this.OnSelectedCommand.Execute(this.SelectedItem);
    }
    private void Search_TextChanged(object? sender, TextChangedEventArgs e)
    {
        string filter = e.NewTextValue;

        if (this.ListView?.ItemsSource is null || this.ItemsSource is null)
            return;

        this.ListView.ItemsSource = string.IsNullOrWhiteSpace(filter)
            ? this.ItemsSource
            : this.ItemsSource.Cast<object?>().Where(o => EntryPicker.ObjectContainsValue(o, filter));
    }
    #endregion Event Handlers

    #region Helpers
    private Grid CreatePopupContent()
    {
        // Main container
        var parent = new Grid()
        {
            ColumnDefinitions = { new(new(1, GridUnitType.Auto)), new(new(1, GridUnitType.Star)) },
            RowDefinitions = { new(new(1, GridUnitType.Auto)), new(new(1, GridUnitType.Star)) },
            Margin = new(0),
            Padding = new(20),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Center,
            BackgroundColor = Colors.Transparent
        };

        // Popup title
        var title = new Label();
        title.SetBinding(Label.TextProperty, new Binding(nameof(this.Title), source: this));
        parent.SetColumn(title, 0);
        parent.SetRow(title, 0);
        parent.Add(title);

        // Close button
        this.ClosePopupCommand = new AsyncRelayCommand(p => this.ClosePopup());
        App.Current!.Resources.TryGetValue("DangerButton", out object buttonDangerStyle);
        App.Current!.Resources.TryGetValue("Xmark", out object xGlyph);

        var closeButton = new Button()
        {
            VerticalOptions = LayoutOptions.Start,
            HorizontalOptions = LayoutOptions.End,
            Style = (Style)buttonDangerStyle,
            Margin = new(0),
            Padding = new(5),
            //FontSize = 16,
            MinimumHeightRequest = 20,
            MinimumWidthRequest = 20,
            ZIndex = 99,
            ImageSource = new FontImageSource()
            {
                Size = 16,
                FontFamily = CommonValues.FaSolidFont,
                Glyph = (string)xGlyph
            }
        };
        closeButton.SetBinding(Button.CommandProperty, new Binding(nameof(this.ClosePopupCommand), source: this));
        parent.SetColumn(closeButton, 1);
        parent.SetRow(closeButton, 0);
        parent.Add(closeButton);

        // Searchbar and Listview container
        var frame = new Frame()
        {
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Fill,
            BackgroundColor= Colors.Transparent
        };
        parent.SetColumn(frame, 0);
        parent.SetRow(frame, 1);
        parent.SetColumnSpan(frame, 3);
        parent.Add(frame);

        // Searchbar and Listview container layout
        var grid = new Grid()
        {
            ColumnDefinitions = { new(new(1, GridUnitType.Star)) },
            RowDefinitions = { new(new(1, GridUnitType.Auto)), new(new(1, GridUnitType.Star)) },
            Margin = new(0, 20, 0, 0),
            Padding = new(10),
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions = LayoutOptions.Fill,
        };
        frame.Content = grid;

        // Searcbar
        this.SearchBar = new SearchBar()
        {
            WidthRequest = 300,
        };
        this.SearchBar.TextChanged += this.Search_TextChanged;
        grid.SetColumn(this.SearchBar, 0);
        grid.SetRow(this.SearchBar, 0);
        grid.Add(this.SearchBar);

        // Listview
        this.ListView = new ListView()
        {
            VerticalOptions = LayoutOptions.Fill,
            HorizontalOptions= LayoutOptions.Fill,
        };
        grid.SetColumn(this.ListView, 0);
        grid.SetRow(this.ListView, 1);
        grid.Add(this.ListView);

        //this.ListView.SetBinding(Label.TextProperty, new Binding(nameof(this.Title), source: this)); // selected item        
        this.ListView.SetBinding(ListView.ItemsSourceProperty, new Binding(nameof(this.ItemsSource), source: this)); // items
        this.ListView.ItemSelected += this.ListView_ItemSelected;

        return parent;
    }

    /// <summary>
    /// Searches and object and detrmines if any of the object's property values contain the value provided using case insensitive search. 
    /// For non-complex objects such as primary data types and strings, searches the ToString value with the value provided. 
    /// Enum values use the enum's Name value.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static bool ObjectContainsValue(object? obj, string value)
    {
        if (obj is null)
            return false;
        Type objType = obj.GetType();

        switch (objType)
        {
            case Type _ when objType == typeof(string):
                return (obj as string)!.Contains(value, StringComparison.InvariantCultureIgnoreCase);
            case Type _ when objType == typeof(int):
            case Type _ when objType == typeof(decimal):
            case Type _ when objType == typeof(float):
            case Type _ when objType == typeof(long):
            case Type _ when objType == typeof(sbyte):
            case Type _ when objType == typeof(byte):
            case Type _ when objType == typeof(short):
            case Type _ when objType == typeof(ushort):
            case Type _ when objType == typeof(uint):
            case Type _ when objType == typeof(ulong):
            case Type _ when objType == typeof(nint):
            case Type _ when objType == typeof(nuint):
            case Type _ when objType == typeof(bool):
            case Type _ when objType == typeof(char):
                return obj.ToString()!.Contains(value, StringComparison.InvariantCultureIgnoreCase);
            case Type _ when objType == typeof(Enum):
                return Enum.GetName(objType, value)!.Contains(value, StringComparison.InvariantCultureIgnoreCase);
            default:
                break;
        }

        foreach (PropertyInfo prop in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (prop.GetIndexParameters().Length > 0) continue;
            if (prop.GetValue(obj, null)?.ToString()?.Contains(value, StringComparison.InvariantCultureIgnoreCase) ?? false)
                return true;
        }

        return false;
    }
    #endregion Helpers
}