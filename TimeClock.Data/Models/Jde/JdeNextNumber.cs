using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Data.Models.Jde;

/// <summary>
/// F0002 | Next Numbers - Automatic
/// </summary>
public partial class JdeNextNumber
{
    [NotMapped]
    public static readonly string PayrollTransactionProductCode = "47";
    /// <summary>
    /// NNSY | Product Code | UDC (98 SY)
    /// </summary>
    /// <remarks>Join to CustomCodes on <see cref="JdeCustomCode.ProductCode"/> == 98 AND <see cref="JdeCustomCode.Codes"/> == 'SY'</remarks>
    public string ProductCode { get; set; } = null!;
    /// <summary>
    /// NNUD01 | Use Description 01
    /// </summary>
    public string? Nnud01 { get; set; }
    /// <summary>
    /// NNUD02 | Use Description 02
    /// </summary>
    public string? Nnud02 { get; set; }
    /// <summary>
    /// NNUD03 | Use Description 03
    /// </summary>
    public string? Nnud03 { get; set; }
    /// <summary>
    /// NNUD04 | Use Description 04
    /// </summary>
    public string? Nnud04 { get; set; }
    /// <summary>
    /// NNUD05 | Use Description 05
    /// </summary>
    public string? Nnud05 { get; set; }
    /// <summary>
    /// NNUD06 | Use Description 06
    /// </summary>
    public string? Nnud06 { get; set; }
    /// <summary>
    /// NNUD07 | Use Description 07
    /// </summary>
    public string? Nnud07 { get; set; }
    /// <summary>
    /// NNUD08 | Use Description 08
    /// </summary>
    public string? Nnud08 { get; set; }
    /// <summary>
    /// NNUD09 | Use Description 09
    /// </summary>
    public string? Nnud09 { get; set; }
    /// <summary>
    /// NNUD10 | Use Description 10
    /// </summary>
    public string? Nnud10 { get; set; }
    /// <summary>
    /// NNN001 | Next Number Range 1
    /// </summary>
    public int? NextNumberRange1 { get; set; }
    /// <summary>
    /// NNN002 | Next Number Range 2
    /// </summary>
    public int? NextNumberRange2 { get; set; }
    /// <summary>
    /// NNN003 | Next Number 003
    /// </summary>
    public int? Nnn003 { get; set; }
    /// <summary>
    /// NNN004 | Next Number 004
    /// </summary>
    public int? Nnn004 { get; set; }
    /// <summary>
    /// NNN005 | Next Number 005
    /// </summary>
    public int? Nnn005 { get; set; }
    /// <summary>
    /// NNN006 | Next Number 006
    /// </summary>
    public int? Nnn006 { get; set; }
    /// <summary>
    /// NNN007 | Next Number 007
    /// </summary>
    public int? Nnn007 { get; set; }
    /// <summary>
    /// NNN008 | Next Number 008
    /// </summary>
    public int? Nnn008 { get; set; }
    /// <summary>
    /// NNN009 | Next Number 009
    /// </summary>
    public int? Nnn009 { get; set; }
    /// <summary>
    /// NNN010 | Next Number 010
    /// </summary>
    public int? Nnn010 { get; set; }
    /// <summary>
    /// NNCK01 | Check Digit 01
    /// </summary>
    public string? Nnck01 { get; set; }
    /// <summary>
    /// NNCK02 | Check Digit 02
    /// </summary>
    public string? Nnck02 { get; set; }
    /// <summary>
    /// NNCK03 | Check Digit 03
    /// </summary>
    public string? Nnck03 { get; set; }
    /// <summary>
    /// NNCK04 | Check Digit 04
    /// </summary>
    public string? Nnck04 { get; set; }
    /// <summary>
    /// NNCK05 | Check Digit 05
    /// </summary>
    public string? Nnck05 { get; set; }
    /// <summary>
    /// NNCK06 | Check Digit 06
    /// </summary>
    public string? Nnck06 { get; set; }
    /// <summary>
    /// NNCK07 | Check Digit 07
    /// </summary>
    public string? Nnck07 { get; set; }
    /// <summary>
    /// NNCK08 | Check Digit 08
    /// </summary>
    public string? Nnck08 { get; set; }
    /// <summary>
    /// NNCK09 | Check Digit 09
    /// </summary>
    public string? Nnck09 { get; set; }
    /// <summary>
    /// NNCK10 | Check Digit 10
    /// </summary>
    public string? Nnck10 { get; set; }

}
