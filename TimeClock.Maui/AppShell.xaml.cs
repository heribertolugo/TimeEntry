using TimeClock.Maui.Helpers;
using TimeClock.Maui.Services;
using TimeClock.Maui.ViewModels;

namespace TimeClock.Maui;

public partial class AppShell : Shell
{
    public AppShell()
    {
        this.InitializeComponent();
        AppShell.RegisterRoutes();
        TheTheme.SetTheme();
        this.SetTabs();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
#if SAMPLEDATA && DEBUG      
        var gen = new SampleDataGenerator(ServiceHelper.GetApiService());
        gen.Generate();  
#endif
    }

    private static void RegisterRoutes()
    {
        // Do not add routing here, use UiPageMeta instead
        foreach (IUiPageMeta meta in UiPageMeta.Metas)
        {
            Routing.RegisterRoute(meta.Route, meta.PageType);
        }
    }

    protected override async void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);

        MainViewModel? viewModel = (Shell.Current?.CurrentPage?.BindingContext) as MainViewModel;
        string? targetPage = args.Current?.Location?.ToString().TrimStart(CommonValues.PathSeparator);

        // viewmodel is null when the page that is appearing is NOT our login page
        if (viewModel == null || string.IsNullOrWhiteSpace(targetPage))
        {
            var app = App.Current as App;

            if (app is not null)
                app.LastActivity = DateTime.Now;

            // remove any previously open screens and force the punch-in page to the stack
            await this.ResetTabStack(args);
            return;
        }

        // sets the properties of the UI in the punch-in screen.
        // target action for button, button text and title
        AppShell.SetLoginViewModel(viewModel, targetPage);
    }

    /// <summary>
    /// Resets the stack of the shell tab being entered, to prevent previously visited page from being accessed
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private async Task ResetTabStack(ShellNavigatedEventArgs args)
    {
        if (args.Source == ShellNavigationSource.ShellSectionChanged)
        {
            foreach (var tab in this.shellTabs.Items.Where(i => i is not null))
            {
                await tab.Navigation.PopToRootAsync(true);
            }
        }
    }

    /// <summary>
    /// Sets the properties for the login page view model, which determines text displayed and page navigated to when clicking the action button
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="targetPage"></param>
    private static void SetLoginViewModel(MainViewModel viewModel, string targetPage)
    {
        IUiPageMeta? pageMeta = UiPageMeta.GetByName(targetPage);

        if (pageMeta is null || pageMeta.RedirectChild is null)
        {
            viewModel.TargetPageDescription = CommonValues.UnsupportedPage;
            viewModel.TargetPagePath = UiPageMeta.Punch.Path;
            viewModel.TargetActionDescription = CommonValues.Retry;
            return;
        }

        viewModel.TargetPageDescription = pageMeta.Title;
        viewModel.TargetPagePath = pageMeta.RedirectChild.Path;
        viewModel.TargetActionDescription = pageMeta.ActionText;
        viewModel.TitleColor = pageMeta.Color;
    }

    /// <summary>
    /// Generates Shell Tabs from UiPageMeta
    /// </summary>
    private void SetTabs()
    {
        Color light = Colors.Black;
        Color dark = Colors.White;

#if UNCONFIGURED && DEBUG // constant defined in csproj -- ensures app always starts requiring setup / not registered
        Settings.Reset();
#endif

        if (App.Current != null && App.Current.Resources.TryGetValue("Black", out var colorvalue1))
            light = (Color)colorvalue1;
        if (App.Current != null && App.Current.Resources.TryGetValue("White", out var colorvalue2))
            dark = (Color)colorvalue2;

        Color iconColor = (Color)new AppThemeBindingExtension() { Light = light, Dark = dark }.Value;

        IEnumerable<IUiPageMeta> pageMetas =
#if FORCELOGIN && DEBUG // constant defined in csproj
            // force login screen always whether or not app is registered
            Helpers.UiPageMeta.Metas.Where(m => m.RedirectChild is not null).OrderBy(m => m.Value);
#else
            // force user to config screen if we do not have an ID for this device
            !Settings.IsRegistered 
            ? Helpers.UiPageMeta.Metas.Where(p => p.Name == UiPageMeta.Registration.Name) 
            : Helpers.UiPageMeta.Metas.Where(m => m.RedirectChild is not null).OrderBy(m => m.Value);
#endif

        foreach (IUiPageMeta pageMeta in pageMetas)
        {
            this.shellTabs.Items.Add(
                new Tab()
                {
                    Title = pageMeta.Name,
                    Icon = new FontImageSource() { FontFamily = CommonValues.FaSolidFont, Color = iconColor, Glyph = pageMeta.IconGlyph },    
                    Items =
                    {
                        new ShellContent()
                        {
                            Route = pageMeta.Route, ContentTemplate = new DataTemplate(pageMeta.PageType)
                        }
                    }
                }
            );
        }
    }
}
