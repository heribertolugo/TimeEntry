namespace TimeClock.Data.Models;

public class JobTypePayRule : IEntityModel
{
    public Guid Id { get; set; }
    public int RowId { get; set; }
    private IList<string>? _objectsCollection;
    /// <summary>
    /// List of semi-colon separated job codes
    /// </summary>
    public string JobTypes { get; set; } = null!;
    public Guid RateIncreaseId { get; set; }
    public Guid? SpecialPayId { get; set; }
    public Guid PaySettingsId { get; set; }

    public virtual PaySettings PaySettings { get; set; } = null!;
    public virtual RateIncrease RateIncrease { get; set; } = null!;
    public virtual SpecialPay? SpecialPay { get; set; }
    public IEnumerable<string> GetJobTypesAsCollection()
    {
        if (this._objectsCollection is null)
            this._objectsCollection = this.JobTypes.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return this._objectsCollection;
    }
}
