using TimeClock.Core.Models;

namespace TimeClock.Maui.Helpers;

internal interface IUiPageMeta 
{
    int Value { get; init; }
    string Name { get; init; }
    string Uri { get; set; }
    string Path { get; }
    string Route { get; }
    string Title { get; init; }
    string ActionText { get; init; }
    IUiPageMeta? RedirectChild { get; init; }
    Type PageType { get; init; }
    string IconGlyph { get; init; }
    Color Color { get; init; }
    IEnumerable<AuthorizationClaimType> Claims { get; init; }
}

/// <summary>
/// Defines and lists common meta properties for pages
/// </summary>
/// <remarks>
/// The reason it was decided to create a flat hierarchy rather than nested children is to keep the ability to reference any UiPageMeta 
/// in a enum like manner.
/// </remarks>
internal sealed class UiPageMeta : IUiPageMeta
{
    private static readonly Dictionary<string, IUiPageMeta> PagesMeta = [];
    private static readonly List<IUiPageMeta> _metas = [];

    //** Main landing pages/children **\\
    public static readonly UiPageMeta PunchDummy = new(-1, "Punch", "punch", typeof(Views.Main));
    public static readonly UiPageMeta JobTypeStepSelect = new(-11, "JobTypeStepSelect", "selectjobtype", typeof(Views.MainSection.SelectJobTypeStep), "Select Job Type");
    public static readonly UiPageMeta EquipmentSelect = new(11, "EquipmentSelect", "select", typeof(Views.SelectEquipment), "Equipment Select", authorizationClaims: [AuthorizationClaimType.CanSelectEquipment]);
    public static readonly UiPageMeta ViewHistory = new(21, "ViewHistory", "view", typeof(Views.History), "History View");
    public static readonly UiPageMeta AdminHome = new(31, "AdminHome", "home", typeof(Views.Admin), "Admin View", authorizationClaims: [AuthorizationClaimType.CanViewOthersPunches, AuthorizationClaimType.CanEditOthersPunches, AuthorizationClaimType.CanCreateEmployee]);
    public static readonly UiPageMeta ConfigHome = new(41, "ConfigHome", "home", typeof(Views.Configurations), "Configurations", authorizationClaims: [AuthorizationClaimType.CanConfigureApp]);
    public static readonly UiPageMeta Registration = new(-2, "Registration", "registration", typeof(Views.Registration), "Setup");
    //** Login pages **\\
    //public static readonly UiPageMeta Punch = new UiPageMeta(0, "Punch", "punch", typeof(Views.Layout), "Time Clock", "Punch In/Out", redirectChild: PunchDummy, iconGlyph: FaSolidGlyphs.Clock, color: Color.FromArgb("ff9500"));
    public static readonly UiPageMeta Punch = new(0, "Punch", "punch", typeof(Views.Main), "Time Clock", "Punch In/Out", redirectChild: PunchDummy, iconGlyph: FaSolidGlyphs.Clock, color: Color.FromArgb("ff9500"));
    public static readonly UiPageMeta Equipment = new(1, "Equipment", "equipment", typeof(Views.Main), "Equipment Select", "Proceed", EquipmentSelect, iconGlyph: FaSolidGlyphs.TruckMonster, authorizationClaims: [AuthorizationClaimType.CanSelectEquipment], color: Color.FromArgb("34c759"));
    public static readonly UiPageMeta History = new(2, "History", "history", typeof(Views.Main), "View History", "View", ViewHistory, iconGlyph: FaSolidGlyphs.ClockRotateLeft, color: Color.FromArgb("007aff"));
    public static readonly UiPageMeta Admin = new(3, "Admin", "admin", typeof(Views.Main), "Administrators", "Verify", AdminHome, iconGlyph: FaSolidGlyphs.UserTie, authorizationClaims: [AuthorizationClaimType.CanViewOthersPunches, AuthorizationClaimType.CanEditOthersPunches, AuthorizationClaimType.CanCreateEmployee], color: Color.FromArgb("d600aa"));
    public static readonly UiPageMeta Config = new(4, "Config", "config", typeof(Views.Main), "Device Config", "Proceed", ConfigHome, iconGlyph: FaSolidGlyphs.Gear, authorizationClaims: [AuthorizationClaimType.CanConfigureApp], color: Color.FromArgb("ff2d55"));


    /// <summary>
    /// Represents details for configuring, loading or setting up a page throughout the applications. 
    /// </summary>
    /// <param name="value">A unique index/id which can be used for ordering</param>
    /// <param name="name">A name which can be used to refer to an instance. Should be the same as the instance name.</param>
    /// <param name="uri">A base value which will be used as the page for the page. Should not contain parent or children. The actual path and routing is calculated from this value.</param>
    /// <param name="pageType">The <c>Type</c> of the xaml page that will be loaded. This value will determine what page is loaded to the provided <paramref name="uri"/></param>
    /// <param name="title">A value which can used as a title for the page</param>
    /// <param name="actionText">Text to use for a button which leaves this page</param>
    /// <param name="redirectChild">A page to target from this page</param>
    /// <param name="iconGlyph">A glyph to use as an icon for this page</param>
    private UiPageMeta(int value, string name, string uri, Type pageType, string title = "", string actionText = "", 
        IUiPageMeta? redirectChild = null, string iconGlyph = "", IEnumerable<AuthorizationClaimType>? authorizationClaims = null, Color? color = null)
    {
        this._uri = this.Path = this.Route = string.Empty; // to make the compiler happy, nothing more
        this.Value = value;
        this.Name = name;
        this.Uri = uri;
        this.Title = title;
        this.ActionText = actionText;
        this.RedirectChild = redirectChild;
        this.PageType = pageType;
        this.IconGlyph = iconGlyph;
        this.Claims = authorizationClaims ?? [AuthorizationClaimType.Unknown];
        this.Color = color ?? Colors.Transparent;

        // we build the uri for parent child paths
        if (redirectChild != null && redirectChild.Uri != this.Uri)
        {
            redirectChild.Uri = $"{this.Uri}/{redirectChild.Uri}";                
        }
        // ToUpper offers better processing speed than ToLower
        string nameInCaps = name.ToUpperInvariant();
        // we store the items in a dictionary so we can retrieve them by name when needed with optimal performance
        if (!UiPageMeta.PagesMeta.ContainsKey(nameInCaps))
        {
            UiPageMeta.PagesMeta.Add(nameInCaps, this);
            UiPageMeta._metas.Add(this);
        }
        else
        {
            // we cant have 2 page with same name, so replace the current with the new
            // this allows us to use the PunchDummy, since that page should not redirect anywhere, but redirect to itself
            UiPageMeta.PagesMeta[nameInCaps] = this;
            int index = ((List<IUiPageMeta>)UiPageMeta._metas).FindIndex(m => m.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (index != -1)
                UiPageMeta._metas[index] = this;
        }
    }
    /// <summary>
    /// An integer value used to uniquely identify a UiPageMeta
    /// </summary>
    public int Value { get; init; }
    /// <summary>
    /// Name of the page. Page names must be unique case insensitive.
    /// </summary>
    public string Name { get; init; }
    private string _uri;
    /// <summary>
    /// The base value used to set the path and route, without slashes, ie: mypage
    /// </summary>
    public string Uri 
    { 
        get => this._uri; 
        set
        {
            this._uri = value;
            this.Path = $"//{this._uri}";
            this.Route = $"{this._uri}";
        }
    }
    /// <summary>
    /// Gets the provided Uri in path format, with double slash at beginning
    /// </summary>
    public string Path { get; private set; }
    /// <summary>
    /// Gets the provided Uri in route format, with single slash separating child pages
    /// </summary>
    public string Route { get; private set; }
    public string Title { get; init; }
    public string ActionText { get; init; }
    public IUiPageMeta? RedirectChild { get; init; }
    /// <summary>
    /// The <code>Type</code> of the xaml page that will be loaded
    /// </summary>
    public Type PageType { get; init; }
    /// <summary>
    /// A resource key ofr an icon glyph
    /// </summary>
    public string IconGlyph { get; init; }

    public Color Color { get; init; }

    public IEnumerable<AuthorizationClaimType> Claims { get; init; }

    public static explicit operator int(UiPageMeta m) => m.Value;
    public static explicit operator UiPageMeta(int value) => (UiPageMeta)Metas.First(m => m.Value == value);

    /// <summary>
    /// An iterable of all UiPageMetas
    /// </summary>
    public static IEnumerable<IUiPageMeta> Metas { get => UiPageMeta._metas; }
    public static IUiPageMeta? GetByName(string name)
    {
        name = name.ToUpperInvariant();
        return !PagesMeta.TryGetValue(name, out IUiPageMeta? value) ? null : value;
    }
}
