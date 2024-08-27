using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TimeClock.Maui.ViewModels.Shared;

namespace TimeClock.Maui.Controls;

public partial class TabControl : ContentView
{
    /// <summary>
    /// Keeps track if we are setting the active tab internally. Used to prevent infinite loops.
    /// </summary>
    private bool settingInternal = false;
    private ObservableCollection<string> _tabNames;
    private ObservableCollection<string> _tabTitles;

    #region Public Bindable Properties
    public static readonly BindableProperty TabsProperty = BindableProperty.Create(nameof(Tabs), typeof(ObservableCollection<TabControlItem>), typeof(TabControl));
    public static readonly BindableProperty SelectedTabNameProperty = BindableProperty.Create(nameof(SelectedTabName), typeof(string), typeof(TabControl));
    public static readonly BindableProperty SelectedTabTitleProperty = BindableProperty.Create(nameof(SelectedTabTitle), typeof(string), typeof(TabControl));
    public static readonly BindableProperty SelectedTabProperty = BindableProperty.Create(nameof(SelectedTab), typeof(TabControlItem), typeof(TabControl));
    public static readonly BindableProperty TabChangedProperty = BindableProperty.Create(nameof(TabChanged), typeof(IAsyncRelayCommand), typeof(TabControlItem));
    #endregion Public Bindable Properties


    public TabControl()
	{
        this.InitializeComponent();
        this._tabNames = new ObservableCollection<string>();
        this.TabNames = new ReadOnlyObservableCollection<string>(this._tabNames);
        this._tabTitles = new ObservableCollection<string>();
        this.TabTitles = new ReadOnlyObservableCollection<string>(this._tabTitles);
        this.Tabs = new ObservableCollection<TabControlItem>();
        this.Tabs.CollectionChanged += this.OnTabsCollectionChanged;
        this.tabControlLayout.BindingContext = this;
        this.content.BindingContext = this;
        this.Loaded += this.TabControl_Loaded;
    }

    #region Public Properties
    public IAsyncRelayCommand TabChanged
    {
        get { return (IAsyncRelayCommand)base.GetValue(TabChangedProperty); }
        set { base.SetValue(TabChangedProperty, value);}
    }
    /// <summary>
    /// Bindable list containing the names in the Tabs collection.
    /// </summary>
    public ReadOnlyObservableCollection<string> TabNames { get; init; }
    /// <summary>
    /// Bindable list containing the titles in the Tabs collection.
    /// </summary>
    public ReadOnlyObservableCollection<string> TabTitles { get; init; }
    /// <summary>
    /// Bindable and modifiable collection of TabControlItem's
    /// </summary>
    public ObservableCollection<TabControlItem> Tabs
    {
        get => (ObservableCollection<TabControlItem>)base.GetValue(TabsProperty);
        private set => base.SetValue(TabsProperty, value);
    }
    /// <summary>
    /// Gets the name of the currently selected tab, or set the currently selected tab by name.
    /// </summary>
    public string SelectedTabName
    {
        get => (string)base.GetValue(SelectedTabNameProperty);
        set
        {
            // update the actual selected tab that matches the name passed in
            // and update the SelectedTabTitle
            if (!this.settingInternal && this.IsLoaded)
            {
                this.settingInternal = true;
                this.SelectedTab = this.Tabs.FirstOrDefault(t => t.Name!.Equals(value));
                if (this.SelectedTab == null)
                    throw new InvalidOperationException(CommonValues.TabSelectedNotExists);
                this.SelectedTabTitle = this.SelectedTab?.Title ?? string.Empty;
                this.settingInternal = false;
                //this.HandleTabChange();
            }
            base.SetValue(SelectedTabNameProperty, value);
        }
    }
    /// <summary>
    /// Gets the title of the currently selected tab, or set the currently selected tab by title.
    /// </summary>
    public string SelectedTabTitle
    {
        get => (string)base.GetValue(SelectedTabTitleProperty);
        set
        {
            if (!this.settingInternal && this.IsLoaded)
            {
                // update the actual selected tab that matches the title passed in
                // and update the SelectedTabName
                this.settingInternal = true;
                this.SelectedTab = this.Tabs.FirstOrDefault(t => t.Title!.Equals(value));
                if (this.SelectedTab == null)
                    throw new InvalidOperationException(CommonValues.TabSelectedNotExists);
                this.SelectedTabName = this.SelectedTab?.Name ?? string.Empty;
                this.settingInternal = false;
                //this.HandleTabChange();
            }
            base.SetValue(SelectedTabTitleProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the currently selected Tab
    /// </summary>
    public TabControlItem? SelectedTab
    {
        get => (TabControlItem?)base.GetValue(SelectedTabProperty);
        set
        {
            if (value is not null && !this.Tabs.Contains(value))
                throw new InvalidOperationException(CommonValues.TabSelectedNotExists);
            // decide how we will select a tab by default if no tab is currently selected.
            // if we have a value for SelectedTabName, use that - it receives highest precedence.
            // otherwise we can try an use SelectedTabTitle value.
            // if both SelectedTabName and SelectedTabTitle have no value, just grab the first tab in collection to select it.
            Func<TabControlItem, bool> predicate = !string.IsNullOrWhiteSpace(this.SelectedTabName)
                ? (t) => t.Name!.Equals(this.SelectedTabName)
                : (!string.IsNullOrWhiteSpace(this.SelectedTabTitle) ? (t) => t.Title!.Equals(this.SelectedTabTitle) : (t) => true);

            var newTab = (value == null && this.Tabs.Count > 0) ? this.Tabs.FirstOrDefault(predicate) : value;

            base.SetValue(SelectedTabProperty, newTab);

            this.SetTitleNameRadio(value);
            this.HandleTabChange();
        }
    }
    #endregion Public Properties

    #region Private Helpers
    /// <summary>
    /// Sets the value for SelectedTabName and SelectedTabTile properties to the value in the TabControlItem parameter. 
    /// Subsequently updates the selected tab in the UI.
    /// </summary>
    /// <param name="value"></param>
    /// <remarks>This can only be called after control has loaded and if settingInternal is not active.</remarks>
    private void SetTitleNameRadio(TabControlItem? value)
    {
        if (!this.settingInternal && this.IsLoaded)
        {
            this.settingInternal = true;
            this.SelectedTabName = this.SelectedTab?.Name ?? string.Empty;
            this.SelectedTabTitle = this.SelectedTab?.Title ?? string.Empty;
            var radioButtons = this.tabControlLayout.Children.OfType<RadioButton>();

            if (radioButtons != null)
                foreach (RadioButton radioButton in radioButtons)
                {
                    radioButton.CheckedChanged -= this.RadioButton_CheckedChanged;
                    radioButton.IsChecked = radioButton.Content.Equals(this.SelectedTabTitle);
                    radioButton.CheckedChanged += this.RadioButton_CheckedChanged;
                }

            this.settingInternal = false;
        }
    }
    private void HandleTabChange()
    {
        if (this.TabChanged != null)
        {
            this.TabChanged.Execute(this.SelectedTab);
            if (this.SelectedTab != null)
            {
                IViewModel? viewModel = this.SelectedTab.Content.BindingContext as IViewModel;
                if (viewModel?.IsNotBusy ?? false)
                    viewModel?.RefreshCommand?.Execute(this);
            }
        }
    }
    #endregion Private Helpers

    #region Event Handlers
    private void OnTabsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
#warning implement changes to tab collection
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                // add a new tab/radio button for each new TabControlItem added
                if (e.NewItems is not null) {
                    foreach (TabControlItem item in e.NewItems)
                    {
                        // check if tab with same title added. if so throw exception
                        if (this.TabNames.Any(t => t.Equals(item.Name)))
                            throw new InvalidOperationException(string.Format(CommonValues.TabAlreadyAdded, item.Name));
                        this._tabNames.Add(item.Name!);
                        this._tabTitles.Add(item.Title!);
                    }
                }
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                break;
            default:
                break;
        }

        // if we don't have a selected tab, force a recheck to set a selected tab if we have any available
        // otherwise we'll keep the previously selected tab, make it the consumer's responsibility to set a different selected tab
        if (this.SelectedTab == null && this.IsLoaded)
            this.SelectedTab = null;
    }

    private void RadioButton_CheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        RadioButton? radio = sender as RadioButton;
        // our tabs in UI are actually RadioButton's masked to look like tabs.
        // when a tab is clicked on, the selected RadioButton changes.
        // when that change occurs, we need to update the UI to display the content corresponding to the selected tab
        if (radio == null)
            return;

        if (e.Value && this.IsLoaded)
            this.SelectedTab = this.Tabs.FirstOrDefault(t => t.Title!.Equals(radio.Content));
    }

    private void TabControl_Loaded(object? sender, EventArgs e)
    {
        // our control has loaded, set the selected tab to first tab
        // this is done in the selected tab property whenever there is no specific tab specified to be active
        // we cannot have a null for a selected tab if we have tabs available.
        // it just wouldn't make sense in UI.
        this.SelectedTab = null;
    }
    #endregion Event Handlers
}
