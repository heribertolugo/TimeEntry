namespace TimeClock.Data.Models;

/// <summary>
/// Specifies the intent behind a <see cref="WorkPeriod"/> object and how its time relates to other <see cref="WorkPeriod"/> items in the dame date for the same user
/// </summary>
public enum WorkPeriodPurpose
{
    /// <summary>
    /// Signifies the <see cref="WorkPeriod"/> is created from and used to track time in a series of <see cref="PunchEntry"/>
    /// </summary>
    PunchEntriesSum,
    /// <summary>
    /// Special time runs concurrent with punch entries.
    /// </summary>
    SpecialTime,
    /// <summary>
    /// Time that is to be in addition to any time from <see cref="PunchEntry"/>
    /// </summary>
    ExtraTime
}
