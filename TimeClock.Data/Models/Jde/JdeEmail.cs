namespace TimeClock.Data.Models.Jde;

public partial class JdeEmail : IJdeEntityModel
{
    /// <summary>
    /// EAAN8 | Address Number
    /// </summary>
    public int EmployeeId { get; set; }
    /// <summary>
    /// EAIDLN | Who's Who Line Number - ID
    /// </summary>

    public int Eaidln { get; set; }
    /// <summary>
    /// EARCK7 | Line Number ID - 5.0
    /// </summary>
    /// <remarks>This ID is unique per User. It is a consecutive number for each of the user's email addresses starting with 1.</remarks>

    public int EmployeeEmailId { get; set; }
    ///// <summary>
    ///// EAETP | Electronic Address Type
    ///// </summary>

    //public string? Eaetp { get; set; }
    /// <summary>
    /// EAEMAL | Electronic Address
    /// </summary>

    public string? Email { get; set; }
    ///// <summary>
    ///// EAUSER | User ID
    ///// </summary>

    //public string? UpdatedBy { get; set; }
    ///// <summary>
    ///// EAPID | Program ID
    ///// </summary>

    //public string? AppId { get; set; }
    ///// <summary>
    ///// EAUPMJ | Date - Updated
    ///// </summary>

    //public int? UpdatedOn { get; set; }
    ///// <summary>
    ///// EAJOBN | Work Station ID
    ///// </summary>

    //public string? Eajobn { get; set; }
    ///// <summary>
    ///// EAUPMT | Time - Last Updated
    ///// </summary>

    //public int? Eaupmt { get; set; }
    ///// <summary>
    ///// EAEHIER | Messaging Indicator
    ///// </summary>

    //public int? Eaehier { get; set; }
    ///// <summary>
    ///// EAEFOR | Email Format
    ///// </summary>

    //public string? Eaefor { get; set; }
    ///// <summary>
    ///// EAECLASS | Electronic Address Category
    ///// </summary>

    //public string? Eaeclass { get; set; }
    ///// <summary>
    ///// EACFNO1 | Generic Number 1
    ///// </summary>

    //public int? Eacfno1 { get; set; }
    ///// <summary>
    ///// EAGEN1 | General Information - 1
    ///// </summary>

    //public string? Eagen1 { get; set; }
    ///// <summary>
    ///// EAFALGE | Future Flag Use
    ///// </summary>

    //public string? Eafalge { get; set; }
    ///// <summary>
    ///// EASYNCS | Synchronization Status
    ///// </summary>

    //public int? Easyncs { get; set; }
    ///// <summary>
    ///// EACAAD | Server Status
    ///// </summary>

    //public int? Eacaad { get; set; }

    public virtual JdeEmployee Employee { get; set; } = null!;
}

