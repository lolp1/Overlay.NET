using System;

namespace Overlay.NET.Common {
    public static class TimeSpanExtensions {
        public static TimeSpan Days(this int days) => new TimeSpan(days, 0, 0, 0);

        public static TimeSpan Hours(this int hours) => new TimeSpan(0, hours, 0, 0);

        public static TimeSpan Minutes(this int minutes) => new TimeSpan(0, 0, minutes, 0);

        public static TimeSpan Seconds(this int seconds) => new TimeSpan(0, 0, 0, seconds);

        public static TimeSpan Milliseconds(this int milliseconds) => new TimeSpan(0, 0, 0, 0, milliseconds);
    }
}