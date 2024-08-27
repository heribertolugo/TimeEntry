namespace TimeClock.Data.Models.Jde;

/// <summary>
/// F1204 | Location Tracking Table
/// </summary>
public partial class JdeEquipmentLocation : IJdeEntityModel
{
    /// <summary>
    /// FMNNBR | Next Number
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// FMNUMB | Asset Item Number
    /// </summary>
    public int EquipmentId { get; set; }
    /// <summary>
    /// FMLOC | Business Unit - Location
    /// </summary>
    public string LocationId { get; set; } = " ";
    /// <summary>
    /// FMAL | Location Code ('C') = Currently at this location
    /// </summary>
    public char CurrentState { get; set; } = ' ';
    /// <summary>
    /// FMAN8 | Address Number
    /// </summary>
    public int AddressNumber { get; set; }
    /// <summary>
    /// FMMCU | Business Unit
    /// </summary>
    public string BusinessUnit { get; set; } = " ";

    public virtual JdeEquipment Equipment { get; set; } = null!;
    public virtual JdeLocation Location { get; set; } = null!;
}
