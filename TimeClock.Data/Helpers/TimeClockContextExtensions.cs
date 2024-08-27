using Microsoft.EntityFrameworkCore;
using TimeClock.Data.Models;

namespace TimeClock.Data.Helpers;
internal static class TimeClockContextExtensions
{
    public static async Task<bool> IsPreviousMissingPunch(this DbSet<WorkPeriod> workPeriods, Guid currentId, Guid userId)
    {
        int rowId = (await workPeriods.FirstOrDefaultAsync(w => w.Id == currentId))?.RowId ?? int.MaxValue;
        return (await workPeriods.OrderByDescending(w => w.RowId)
            .Where(w => w.UserId == userId && w.RowId < rowId && w.Purpose == WorkPeriodPurpose.PunchEntriesSum)
            .Select(w => w.PunchEntries.Count)
            .FirstOrDefaultAsync()) % 2 != 0;
    }
    public static async Task<bool> IsPreviousMissingPunch(this DbSet<WorkPeriod> workPeriods, WorkPeriod current)
    {
        int rowId = current.RowId == 0 ? int.MaxValue : current.RowId;
        //var q = workPeriods.OrderByDescending(w => w.RowId).Where(w => w.UserId == current.UserId && w.RowId < rowId).Select(w => w.PunchEntries.Count);
        return (await workPeriods.OrderByDescending(w => w.RowId)
            .Where(w => w.UserId == current.UserId && w.RowId < rowId && w.Purpose == WorkPeriodPurpose.PunchEntriesSum)
            .Select(w => w.PunchEntries.Count)
            .FirstOrDefaultAsync()) % 2 != 0;
    }
}
