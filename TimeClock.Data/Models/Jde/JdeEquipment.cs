namespace TimeClock.Data.Models.Jde;

/// <summary>
/// F1201 | Asset Master File
/// </summary>
public partial class JdeEquipment : IJdeEntityModel
{
    /// <summary>
    /// FANUMB | Asset Item Number
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// FAAPID | Unit or Tag Number
    /// </summary>
    public string EquipmentNumber { get; set; } = " ";
    /// <summary>
    /// FADL01 | Description
    /// </summary>
    public string Description1 { get; set; } = " ";
    /// <summary>
    /// FADL02 | Description 02
    /// </summary>
    public string Description2 { get; set; } = " ";
    /// <summary>
    /// FADL03 | Description 03
    /// </summary>
    public string Description3 { get; set; } = " ";
    /// <summary>
    /// FAACL2 | Major Equipment Class.
    /// <para>To get EquipmentType, JOIN <see cref="FK"/> to <see cref="JdeCustomCode.Code"/> (F0005.DRKY) WHERE <see cref="JdeCustomCode.Codes"/> (F0005.DRRT) = 'C2' AND <see cref="JdeCustomCode.ProductCode"/> (F0005.DRSY) = '12'</para>
    /// </summary>
    public string FK { get; set; } = " ";
    /// <summary>
    /// FAEQST | Equipment Status ('AV' == active)
    /// </summary>
    public string Status { get;set; } = " ";

    public virtual ICollection<JdeEquipmentLocation> EquipmentLocations { get; set; } = new List<JdeEquipmentLocation>();
    public virtual JdeCustomCode EquipmentType { get; set; } = null!;
}
