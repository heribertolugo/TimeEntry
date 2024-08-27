namespace TimeClock.Maui.Models
{
    public struct Time : IEquatable<Time>, IComparable<Time>
    {
        private DateTime _dateTime;
        public Time(DateTime dateTime) 
        { 
            this._dateTime = dateTime;
        }
        public Time(TimeSpan timeSpan)
        {
            this._dateTime = new DateTime().Add(timeSpan);
        }
        public Time(TimeOnly timeOnly)
        {
            this._dateTime = new DateTime().Add((Time)timeOnly);
        }
        public Time(int hours, int minutes, int seconds) 
        { 
            this._dateTime = new DateTime().Add(new TimeSpan(hours, minutes, seconds));
        }

        public int Hours { get => this._dateTime.Hour; }
        public int Minutes { get => this._dateTime.Minute; }
        public int Seconds { get => this._dateTime.Second; }

        public override string ToString()
        {
            return this._dateTime.ToString("hh:mm tt");
        }

        public override bool Equals(object? obj)
        {
            return obj is Time time &&
                   Hours == time.Hours &&
                   Minutes == time.Minutes
                   //&& Seconds == time.Seconds
                   ;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Hours, Minutes, Seconds);
        }

        public bool Equals(Time other)
        {
            return this.GetHashCode() == other.GetHashCode();
        }

        public int CompareTo(Time other)
        {
            return this._dateTime.TimeOfDay.CompareTo(other._dateTime.TimeOfDay);
        }

        public static bool operator !=(Time t1, Time t2) => !(t1 == t2);
        public static bool operator ==(Time t1, Time t2) => (t1.Hours == t2.Hours && t1.Minutes == t2.Minutes);

        public static implicit operator TimeSpan(Time t) => t._dateTime.TimeOfDay;
        public static implicit operator Time(TimeSpan t) => new Time(t);
        public static implicit operator TimeOnly(Time t) => new TimeOnly(t.Hours, t.Minutes);
        public static implicit operator Time(TimeOnly t) => new Time(t);
    }
}
