using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
namespace TimeClock.Core.Models.EntityDtos;

public enum WorkPeriodPurposeDto
{
    /// <summary>
    /// Signifies the <see cref="WorkPeriodDto"/> is created from and used to track time in a series of <see cref="PunchEntryDto"/>
    /// </summary>
    PunchEntriesSum,
    /// <summary>
    /// Special time runs concurrent with punch entries.
    /// </summary>
    SpecialTime,
    /// <summary>
    /// Time that is to be in addition to any time from <see cref="PunchEntryDto"/>
    /// </summary>
    ExtraTime
}
