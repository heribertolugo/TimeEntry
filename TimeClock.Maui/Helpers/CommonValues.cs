using System.Collections.Frozen;
using TimeClock.Maui.Helpers;

namespace TimeClock.Maui;

/// <summary>
/// Holds Global read only Values.
/// </summary>
public static class CommonValues
{
    /// <summary>
    /// The delay in milliseconds for cancellation tokens in API calls
    /// </summary>
    public static int ApiCancellationTokenLimit => 5000;
    /// <summary>
    /// The default delay in milliseconds for a cancellation token.
    /// </summary>
    public static int CancellationTokenLimit => 5000;
    /// <summary>
    /// OpenSansRegular
    /// </summary>
    public static string OpenSansRegularFont => "OpenSansRegular";
    /// <summary>
    /// OpenSansSemibold
    /// </summary>
    public static string OpenSansSemiboldFont => "OpenSansSemibold";
    /// <summary>
    /// FaSolid
    /// </summary>
    public static string FaSolidFont => "FaSolid";
    /// <summary>
    /// FaRegular
    /// </summary>
    public static string FaRegularFont => "FaRegular";
    /// <summary>
    /// FaBrand
    /// </summary>
    public static string FaBrandFont => "FaBrand";
    /// <summary>
    /// og256.png
    /// </summary>
    public static string CompanyLogoUrl => "og256.png";
    /// <summary>
    /// '/'
    /// </summary>
    public static char PathSeparator => '/';
    /// <summary>
    /// SystemOrange
    /// </summary>
    public static string SystemOrange => "SystemOrange";
    /// <summary>
    /// #FFFF9500
    /// </summary>
    public static string SystemOrangeHex => "#FFFF9500";
    /// <summary>
    /// Retry
    /// </summary>
    public static string Retry => "Retry";
    /// <summary>
    /// No History
    /// </summary>
    public static string NoHistory => "No History";
    /// <summary>
    /// click a time entry to edit
    /// </summary>
    public static string ClickTimeEntryEdit => "click a time entry to edit";
    /// <summary>
    /// Currently editing {0} entry
    /// </summary>
    public static string CurrEditEntry => "Currently editing {0} entry";
    /// <summary>
    /// Currently creating {0} entry
    /// </summary>
    public static string CurrNewEntry => "Currently creating {0} entry";
    /// <summary>
    /// please wait
    /// </summary>
    public static string PleaseWait => "please wait";
    /// <summary>
    /// you may rezoom
    /// </summary>
    public static string YouMayRezoom => "you may rezoom";
    /// <summary>
    /// invalid user
    /// </summary>
    public static string InvalidUser => "invalid user";
    /// <summary>
    /// Success
    /// </summary>
    public static string Success => "Success";
    /// <summary>
    /// Error
    /// </summary>
    public static string Error => "Error";

    #region Alert Messages
    /// <summary>
    /// Info
    /// </summary>
    public static string Info => "Info";
    /// <summary>
    /// Application is busy. Please try again.
    /// </summary>
    public static string AppBusy => "Application is busy. Please try again.";
    /// <summary>
    /// App.Current is inaccessible
    /// </summary>
    public static string AppCurInac => "App.Current is inaccessible";
    /// <summary>
    /// We could not create the new user.
    /// Try again later or contact help desk.
    /// </summary>
    public static string NotCreateUser => "We could not create the new user.\nTry again later or contact help desk.";
    /// <summary>
    /// We could not create the barcode.
    /// Try again later or contact help desk.
    /// </summary>
    public static string NotCreateBarcode => "We could not create the barcode.\nTry again later or contact help desk.";
    /// <summary>
    /// You have not selected equipment
    /// </summary>
    public static string EquipNotSelect => "You have not selected equipment";
    /// <summary>
    /// Equipment View was not accessible
    /// </summary>
    public static string EquipViewInac => "Equipment View was not accessible";

    public static string GeoLocRequired => "Geolocation is required for this device.\nPlease allow location access in order to continue.";
    #endregion Alert Messages

    #region Exceptions
    /// <summary>
    /// TabControlItem selected does not exist
    /// </summary>
    public static string TabSelectedNotExists => "TabControlItem selected does not exist";
    /// <summary>
    /// A TabControlItem has already been added with the name {0}
    /// </summary>
    public static string TabAlreadyAdded => "A TabControlItem has already been added with the name {0}";
    /// <summary>
    /// ERROR: Unsupported Page
    /// </summary>
    public static string UnsupportedPage => "ERROR: Unsupported Page";
    /// <summary>
    /// Fatal {0} not found
    /// </summary>
    public static string FatalNotFound => "Fatal {0} not found";
    /// <summary>
    /// An exception was encountered getting or setting equipment history
    /// </summary>
    public static string ErrGetSetEquipHist => "An exception was encountered getting or setting equipment history";
    /// <summary>
    /// punch entry cannot be null
    /// </summary>
    public static string PunchEntryNull => "punch entry cannot be null";
    /// <summary>
    /// could not find punch entry. application is in invalid state.
    /// </summary>
    public static string PunchNotFndInvalState => "could not find punch entry. application is in invalid state.";
    /// <summary>
    /// An exception was encountered getting or setting punch entries history
    /// </summary>
    public static string ErrGetSetPunchHist => "An exception was encountered getting or setting punch entries history";
    /// <summary>
    /// API service was not loaded
    /// </summary>
    public static string ApiNotLoaded => "API service was not loaded";
    #endregion Exceptions

    //public static Microsoft.Maui.Devices.Sensors.Location DefaultGeoLocation => new(-90, 0);
    /// <summary>
    /// Key is non-trimmed JDE ID, value is FA namespace and then icon class
    /// </summary>
    public static FrozenDictionary<string, KeyValuePair<string,string>> JobTypeToFaIconMap => new Dictionary<string, KeyValuePair<string, string>>()
    {
        {"    3001  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FilterCircleDollar)}, // Architectural Sales
        {"    1206  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FileContract)}, // Contract Specialist
        {"    1103  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FileInvoiceDollar)}, // Cost Accountant
        {"    2311  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Dungeon)}, // Mason Manager
        {"    2608  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Vault)}, // Treasurer
        {"    1925  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BusinessTime)}, // Subcontractor Coordinator
        {"    UB070 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Hammer)}, // Ironworkers - Building
        {"    1401  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Snowplow)}, // Operator
        {"    2730  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CompassDrafting)}, // Project Engineer
        {"    2002  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PeoplePulling)}, // Project Foreman
        {"    2750  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FileContract)}, // MEP Coordinator
        {"    3401  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.GroupArrowsRotate)}, // Facilty Superinendent
        {"    2607  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Stapler)}, // Secretary
        {"    UB040 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Pencil)}, // Carpenters - Building
        {"    2731  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.SheetPlastic)}, // Jr Project Engineer
        {"    2318  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.RankingStar)}, // Building Marketing Manager
        {"    1203  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.WandMagicSparkles)}, // A/R Team Lead
        {"    2799  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ArrowsSpin)}, // Intern - Project
        {"    2609  ", new KeyValuePair<string, string>(CommonValues.FaBrandFont, FaBrandGlyphs.PhoenixFramework)}, // Vice Chairman
        {"    2310  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FilePrescription)}, // Insurance Manager
        {"    UR070 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.RoadLock)}, // Ironworkers - Road
        {"    1214  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HandHoldingHand)}, // HR Specialist
        {"    1703  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Bullseye)}, // Mason Dispatcher
        {"    1901  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BuildingCircleCheck)}, // Estimator - Building
        {"    2999  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.SquarePersonConfined)}, // Intern - QA/QC
        {"    2613  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PeopleGroup)}, // HR Director
        {"    3006  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Shop)}, // Mason Showroom Sales
        {"    UB030 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TrowelBricks)}, // Laborer - Building
        {"    1399  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Timeline)}, // Intern - Business Systems
        {"    1301  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CodePullRequest)}, // Analyst
        {"    UB830 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Mound)}, // Laborer - Building Mk
        {"    2201  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ToiletPortable)}, // Maintenance Specialist
        {"    1911  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TentArrowTurnLeft)}, // Jr Estimator - Building
        {"    2770  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ArrowUpFromGroundWater)}, // Project Superintendent
        {"    1112  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FileInvoiceDollar)}, // Financial Operations Manager
        {"    1101  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Book)}, // Bookkeeper
        {"    2306  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FileInvoice)}, // Contract Manager
        {"    1204  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Paperclip)}, // A/S Assistant
        {"    URA50 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.VectorSquare)}, // Masons - Zone A - Road
        {"    1400  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.RulerCombined)}, // Carpenter
        {"    2722  ", new KeyValuePair<string, string>(CommonValues.FaBrandFont, FaBrandGlyphs.Connectdevelop)}, // Sr Project Manager
        {"    2308  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Person)}, // Facilty Foreman
        {"    2403  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Globe)}, // Marketing & PreCon Director
        {"    1213  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UserDoctor)}, // Benefit Specialist
        {"    2202  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.MountainCity)}, // Facility Maintenance Specialis
        {"    1205  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Ring)}, // Administrative Asst
        {"    1104  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.MoneyBillTrendUp)}, // Financial Analyst
        {"    1111  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CashRegister)}, // Accountant
        {"    UP010 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Industry)}, // Operator - Plant
        {"    1110  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.MagnifyingGlassDollar)}, // Sr Accountant
        {"    3007  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Tags)}, // Material Sales
        {"    UP020 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HillAvalanche)}, // Teamsters - Plant
        {"    2314  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.MoneyCheckDollar)}, // Payroll Manager
        {"    1704  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TruckRampBox)}, // Scale Dispatcher
        {"    2801  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UserTag)}, // Facility Purchasing Agent
        {"    UN045 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Shapes)}, // Millwright
        {"    1210  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ChartLine)}, // Marketing Specialist
        {"    2604  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ChessKnight)}, // Chief Information Officer
        {"    2760  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CalendarCheck)}, // Project Scheduler
        {"    1102  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CircleDollarToSlot)}, // Controller
        {"    2701  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Tornado)}, // Pre Construction PM
        {"    U9010 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.WaveSquare)}, // GENERIC - Key Actual Step
        {"    2803  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ShopLock)}, // Mason Purchasing Manager
        {"    1912  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TrafficLight)}, // Jr Estimator - Road
        {"    1106  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Percent)}, // Tax Manager
        {"          ", new KeyValuePair<string, string>(CommonValues.FaRegularFont, FaRegularGlyphs.CircleCheck)}, // .
        {"    UB010 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BuildingUser)}, // Operator - Building
        {"    2301  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FolderTree)}, // A/P Manager
        {"    2605  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UserTie)}, // Executive Vice President
        {"    3101  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BezierCurve)}, // Scale Associate
        {"    2732  ", new KeyValuePair<string, string>(CommonValues.FaBrandFont, FaBrandGlyphs.Codepen)}, // Sr Project Engineer
        {"    1215  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CreditCard)}, // Sr Credit Specialist
        {"    1921  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TreeCity)}, // Sr Estimator - Building
        {"    1201  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.EnvelopesBulk)}, // A/P Specialist
        {"    1599  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UserShield)}, // Intern - Safety
        {"    1922  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.RoadBarrier)}, // Sr Estimator - Road
        {"    2720  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HandsHoldingCircle)}, // Project Manager
        {"    2001  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ArrowsDownToPeople)}, // Mason Foreman
        {"    1308  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Computer)}, // Help Desk Manager
        {"    2616  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Bolt)}, // Chief Risk Officer
        {"    2901  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Cubes)}, // QA/QC Engineer
        {"    1307  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.LaptopCode)}, // Help Desk Sr Technician
        {"    2902  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PersonShelter)}, // QA/QC Manager
        {"    2615  ", new KeyValuePair<string, string>(CommonValues.FaBrandFont, FaBrandGlyphs.Hornbill)}, // Deputy Chief Financial Officer
        {"    2317  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.LaptopFile)}, // IT Resources Manager
        {"    1505  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BuildingShield)}, // Safety Manager
        {"    2602  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UserNinja)}, // Assistant Vice President
        {"    1999  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.SquareRootVariable)}, // Intern - Estimating
        {"    1402  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CarSide)}, // Driver
        {"    3402  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ArrowUpFromGroundWater)}, // Project Superintendent
        {"    3002  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Store)}, // Mason Counter Sales
        {"    1202  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Stamp)}, // A/R Specialist
        {"    2313  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Box)}, // Parts Manager
        {"    1109  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Calculator)}, // Accounting Assistant
        {"    2601  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PeopleArrows)}, // Asst Secretary
        {"    1403  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PersonDigging)}, // Laborer
        {"    UR040 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.RulerHorizontal)}, // Carpenters - Road
        {"    2312  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PersonChalkboard)}, // Office Manager
        {"    UR020 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TruckMoving)}, // Trailer 40 Ton & Over
        {"    2343  ", new KeyValuePair<string, string>(CommonValues.FaBrandFont, FaBrandGlyphs.Simplybuilt)}, // Asst Mason Manager
        {"    1507  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HelmetSafety)}, // Safety Specialist
        {"    2611  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Gavel)}, // General Counsel
        {"    2307  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.MoneyBillTransfer)}, // Credit Manager
        {"    2299  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Toolbox)}, // Intern - Maintenance
        {"    3004  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Gopuram)}, // Mason Outside Sales
        {"    1503  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ScaleBalanced)}, // Legal Compliance
        {"    2103  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Tents)}, // Mason Inventory Control
        {"    1209  ", new KeyValuePair<string, string>(CommonValues.FaRegularFont, FaRegularGlyphs.RectangleList)}, // Equipment Records Coord Ast
        {"    2302  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ChalkboardUser)}, // A/R Manager
        {"    1702  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Bullhorn)}, // Labor Dispatcher
        {"    2740  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Landmark)}, // Project Executive
        {"    2203  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TentArrowsDown)}, // Mason Maintenance Specialist
        {"    1506  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ShieldHalved)}, // Safety Director
        {"    UR030 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Bucket)}, // Laborer - Road
        {"    UBA50 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TowerObservation)}, // Masons - Zone A - Building
        {"    1208  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.RectangleList)}, // Equipment Records Coordinator
        {"    1306  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TowerBroadcast)}, // Telecommunications Specialist
        {"    2342  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CartFlatbed)}, // Asst Facility Manager
        {"    1404  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Screwdriver)}, // Mechanic
        {"    2304  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Table)}, // Accounting Manager
        {"    2775  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Mask)}, // Field Supervisor
        {"    1601  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.EnvelopesBulk)}, // A/S Courier
        {"    2305  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UserNurse)}, // Benefit Manager
        {"    UV010 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PersonWalkingWithCane)}, // Operator - Surveyors
        {"    2309  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Warehouse)}, // Facilty Manager
        {"    1902  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.RoadBridge)}, // Estimator - Road
        {"    2316  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.TowerCell)}, // Telecommunications Manager
        {"    UBB50 ", new KeyValuePair<string, string>(CommonValues.FaRegularFont, FaRegularGlyphs.ObjectUngroup)}, // Masons - Zone B - Building
        {"    2315  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HouseChimneyUser)}, // Property Manager
        {"    UR010 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Road)}, // Operator - Road
        {"    2101  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BoxesStacked)}, // Facility Inventory Control
        {"    1302  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CodeCompare)}, // Sr Analyst
        {"    1502  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Biohazard)}, // Environmental Compliance
        {"    2606  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Crown)}, // President
        {"    2610  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Dragon)}, // Vice President
        {"    3005  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.MoneyBills)}, // Mason Sales
        {"    2780  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PeoplePulling)}, // Project Foreman
        {"    2501  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Screwdriver)}, // Mechanic
        {"    UB020 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UsersBetweenLines)}, // Teamsters - Building
        {"    1108  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.MoneyBillWheat)}, // Assistant Controller
        {"    2612  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Seedling)}, // Plant & Permit Director
        {"    2401  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ChartColumn)}, // Marketing Director
        {"    2341  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Wallet)}, // Assistant Credit Manager
        {"    2603  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.SackDollar)}, // Chief Financial Officer
        {"    1304  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.NetworkWired)}, // Sr Network Engineer
        {"    1501  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BookSkull)}, // Compliance Specialist
        {"    2721  ", new KeyValuePair<string, string>(CommonValues.FaRegularFont, FaRegularGlyphs.LifeRing)}, // Jr Project Manager
        {"    1299  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HandshakeAngle)}, // Intern - Admin
        {"    UB045 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.BuildingWheat)}, // Millwright - Building
        {"    2614  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PiggyBank)}, // Assistant Treasurer
        {"    2102  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.ToriiGate)}, // Gate Attendant
        {"    2710  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Cube)}, // Program Manager
        {"    1207  ", new KeyValuePair<string, string>(CommonValues.FaRegularFont, FaRegularGlyphs.CreditCard)}, // Credit Specialist
        {"    1303  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.DiagramProject)}, // Network Engineer
        {"    1212  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.CircleNodes)}, // Project Specialist
        {"    US010 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PeopleRoof)}, // Operator - Shop
        {"    1211  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HandHoldingDollar)}, // Payroll Specialist
        {"    3003  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HouseFlag)}, // Mason Detailer
        {"    UM020 ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Kaaba)}, // Teamsters - Mason
        {"    2802  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.UserSecret)}, // Mason Purchasing Agent
        {"    1107  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Receipt)}, // Jr Accountant
        {"    1305  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Terminal)}, // Help Desk Technician
        {"    2402  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.PersonRays)}, // Mason Marketing Director
        {"    1504  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.FileSignature)}, // Plant & Permit Compliance
        {"    2303  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.HandHoldingHand)}, // A/S Manager
        {"    1105  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Coins)}, // Project Accountant
        {"    1701  ", new KeyValuePair<string, string>(CommonValues.FaSolidFont, FaSolidGlyphs.Trowel)}, // Concrete Dispatcher
    }.ToFrozenDictionary();
}
