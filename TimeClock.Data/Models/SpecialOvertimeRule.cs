namespace TimeClock.Data.Models;

public class SpecialOvertimeRule : IEntityModel
{
    private IList<string>? _objectsCollection;
    public Guid Id { get; set; }
    public int RowId { get; set; }
    /// <summary>
    /// List of semi-colon separated projects or unions
    /// </summary>
    public string Objects { get; set; } = null!;
    /// <summary>
    /// Number of hours to threshold after which overtime will begin
    /// </summary>
    public decimal AfterHours { get; set; }
    public OvertimeThresholdType OvertimeThreshold { get; set; }
    public SpecialOvertimeObjectType ObjectsType { get; set; }
    public Guid? SpecialPayId { get; set; }
    public Guid PaySettingsId { get; set; }

    public virtual PaySettings PaySettings { get; set; } = null!;
    public virtual SpecialPay? SpecialPay { get; set; }
    public IEnumerable<string> GetObjectsAsCollection()
    {
        if (this._objectsCollection is null)
            this._objectsCollection = this.Objects.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return this._objectsCollection;
    }
}

public enum OvertimeThresholdType
{
    Day,
    Week
}

public enum SpecialOvertimeObjectType
{
    Unions,
    Projects
}
