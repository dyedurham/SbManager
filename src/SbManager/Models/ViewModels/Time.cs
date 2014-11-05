using System;

namespace SbManager.Models.ViewModels
{
    public class Time
    {
        public Time() { }

        public Time(TimeSpan tspan)
        {
            Days = Convert.ToInt32(Math.Floor(tspan.TotalDays));
            Hours = tspan.Hours;
            Minutes = tspan.Minutes;
            Seconds = tspan.Seconds;
            MilliSeconds = tspan.Milliseconds;
        }

        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public int MilliSeconds { get; set; }

        public string Human
        {
            get { return ToString(); }
        }

        public override string ToString()
        {
            return string.Format("{0}d {1}h {2}m {3}s {4}ms", Days, Hours, Minutes, Seconds, MilliSeconds);
        }
    }
}
