using TimeClock.Data.Models;

namespace TimeClock.Data.Helpers;

public static class PunchEntryExtensions
{
    public static TimeSpan TotalPunchTime(this IEnumerable<PunchEntriesCurrentState> punchEntries)
    {
        //var punches = punchEntries.Where(p => PunchActionEx.StableStates.Contains(p.PunchEntriesHistory.Action)).OrderBy(e => e.PunchEntriesHistory.EffectiveDateTime).Chunk(2);
        var punches = punchEntries.Where(p => p.StablePunchEntriesHistory != null).OrderBy(e => e.StablePunchEntriesHistory.EffectiveDateTime).Chunk(2);
        TimeSpan total = new ();

        foreach (var chunk in punches)
        {
            if (chunk.Length < 2)
                break;

            total = total.Add(chunk[1].StablePunchEntriesHistory.EffectiveDateTime?.Subtract(chunk[0].StablePunchEntriesHistory.EffectiveDateTime ?? default) ?? default);
        }

        return total;
    }
    public static TimeSpan TotalPunchTime(this IEnumerable<PunchEntry> punchEntries)
    {
        //var punches = punchEntries.Where(p => PunchActionEx.StableStates.Contains(p.CurrentState.PunchEntriesHistory.Action)).OrderBy(e => e.CurrentState.PunchEntriesHistory.EffectiveDateTime).Chunk(2);
        var punches = punchEntries.Where(p => p.CurrentState.StablePunchEntriesHistory != null).OrderBy(e => e.CurrentState.StablePunchEntriesHistory.EffectiveDateTime).Chunk(2);
        TimeSpan total = new();

        foreach (var chunk in punches)
        {
            if (chunk.Length < 2)
                break;

            total = total.Add(chunk[1].CurrentState.StablePunchEntriesHistory.EffectiveDateTime?.Subtract(chunk[0].CurrentState.StablePunchEntriesHistory.EffectiveDateTime ?? default) ?? default);
        }

        return total;
    }
}
