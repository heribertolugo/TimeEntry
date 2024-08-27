using System.ComponentModel.DataAnnotations;

namespace TimeClock.Data.Models.Jde;

/// <summary>
/// F0005 | User Defined Code Values
/// </summary>
public partial class JdeCustomCode : IJdeEntityModel
{
    [MaxLength(4)]
    /// <summary>
    /// Drsy | Product Code
    /// </summary>
    public string ProductCode { get; set; } = null!;
    [MaxLength(2)]
    /// <summary>
    /// Drrt | User Defined Codes
    /// </summary>
    public string Codes { get; set; } = null!;
    [MaxLength(10)]
    /// <summary>
    /// Drky | User Defined Code. 
    /// This is the value, and used in other tables. 
    /// Because it is used as foreign key, this should be trimmed.
    /// </summary>
    public string Code { get; set; } = null!;
    [MaxLength(30)]
    /// <summary>
    /// Drdl01 | Description
    /// </summary>
    public string? Description { get; set; }
    [MaxLength(30)]
    /// <summary>
    /// Drdl02 | Description 02
    /// </summary>
    public string? Drdl02 { get; set; }
    [MaxLength(10)]
    /// <summary>
    /// Drsphd | Special Handling Code - User Def Codes
    /// </summary>
    public string? Drsphd { get; set; }
    [MaxLength(1)]
    /// <summary>
    /// Drudco | Ownership Flag - User Defined Codes (char(1))
    /// </summary>
    public string? Drudco { get; set; }
    [MaxLength(1)]
    /// <summary>
    /// Drhrdc | Hard Coded Y/N (char(1))
    /// </summary>
    public string? Drhrdc { get; set; }
    [MaxLength(10)]
    /// <summary>
    /// Druser | User ID
    /// </summary>
    public string? Druser { get; set; }
    [MaxLength(10)]
    /// <summary>
    /// Drpid | Program ID
    /// </summary>
    public string? Drpid { get; set; }
    [MaxLength(6)]
    /// <summary>
    /// Drupmj | Date - Updated
    /// </summary>
    public int? Drupmj { get; set; }
    [MaxLength(10)]
    /// <summary>
    /// Drjobn | Work Station ID
    /// </summary>
    public string? Drjobn { get; set; }
    [MaxLength(6)]
    /// <summary>
    /// Drupmt | Time - Last Updated
    /// </summary>
    public decimal? Drupmt { get; set; }
}
