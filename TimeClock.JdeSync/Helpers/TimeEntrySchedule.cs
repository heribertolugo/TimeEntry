using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeClock.JdeSync.Helpers;
internal class TimeEntrySchedule
{
    [Required]
    public int Interval { get; set; }
    [Required]
    public IntervalType IntervalType { get; set; }
    [Required]
    public DateTime BeginDate { get; set; }
    [Required]
    public TimeSpan BeginTime { get; set; }
    [Required]
    public int MinutesBuffer { get; set; }
    public DateTime BeginDateTime
    {
        get => this.BeginDate.Add(this.BeginTime);
    }

    public DateTime GetNextRunFromLastRunDate(DateTime lastRunDateTime)
    {
        DateTime nextRun = lastRunDateTime;
        TimeSpan runningInterval = this.GetInterval();
        DateTime now = DateTime.Now;

        while ((nextRun = nextRun.Add(runningInterval)) < now);

        return nextRun;
    }

    private TimeSpan GetInterval()
    {
        switch (this.IntervalType)
        {
            case IntervalType.Hours:
                return new TimeSpan(this.Interval, 0, 0);
            case IntervalType.Days:
                return new TimeSpan(this.Interval, 0,0,0);
            default:
                throw new Exception($"{this.IntervalType} is an invalid {nameof(this.IntervalType)}");
        }
    }
}

internal enum IntervalType
{
    Hours,
    Days
}
