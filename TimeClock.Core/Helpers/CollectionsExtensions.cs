using System.Runtime.CompilerServices;
using TimeClock.Core.Models.EntityDtos;

[assembly: InternalsVisibleTo("TimeClock.Api")]
[assembly: InternalsVisibleTo("TimeClock.Maui")]
[assembly: InternalsVisibleTo("TimeClock.JdeSync")]

namespace TimeClock.Core;

public static class CollectionsExtensions
{
    public static TimeSpan TotalStablePunchTime(this IEnumerable<PunchEntryDto> punchEntries)
    {
        var punches = punchEntries.Where(p => p.StableAction.HasValue && PunchActionDtoEx.PositiveStates().Contains(p.StableAction.Value)).OrderBy(e => e.StableEffectiveDateTime).Chunk(2);
        TimeSpan total = new();

        foreach (var chunk in punches)
        {
            if (chunk.Length < 2)
                break;

            if (chunk[0].StableEffectiveDateTime.HasValue && chunk[1].StableEffectiveDateTime.HasValue)
                total = total.Add(chunk[1].StableEffectiveDateTime!.Value.Subtract(chunk[0].StableEffectiveDateTime!.Value));
        }

        return total;
    }
    public static TimeSpan TotalPunchTime(this IEnumerable<PunchEntryDto> punchEntries)
    {
        var punches = punchEntries.Where(p => PunchActionDtoEx.PositiveStates().Contains(p.Action)).OrderBy(e => e.EffectiveDateTime).Chunk(2);
        TimeSpan total = new();

        foreach (var chunk in punches)
        {
            if (chunk.Length < 2)
                break;

            total = total.Add(chunk[1].EffectiveDateTime.Subtract(chunk[0].EffectiveDateTime));
        }

        return total;
    }

    public static T? TryGetAtIndex<T>(this IList<T> values, int index)
    {
        return values.Count >= index ? default : values[index];
    }
    public static T? TryGetAtIndex<T>(this T[] values, int index)
    {
        return values.Length >= index ? default : values[index];
    }
}
