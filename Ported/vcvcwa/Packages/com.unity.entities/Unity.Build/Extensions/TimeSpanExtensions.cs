using System;
using System.Globalization;

namespace Unity.Build
{
    internal static class TimeSpanExtensions
    {
        public static string ToShortString(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalSeconds < 1.0)
            {
                return timeSpan.Milliseconds.ToString(CultureInfo.InvariantCulture) + "ms";
            }
            else if (timeSpan.TotalMinutes < 1.0)
            {
                return timeSpan.TotalSeconds.ToString("F2", CultureInfo.InvariantCulture) + "s";
            }
            else if (timeSpan.TotalHours < 1.0)
            {
                return timeSpan.TotalMinutes.ToString("F2", CultureInfo.InvariantCulture) + "m";
            }
            else if (timeSpan.TotalDays < 1.0)
            {
                return timeSpan.TotalHours.ToString("F2", CultureInfo.InvariantCulture) + "h";
            }
            else
            {
                return timeSpan.TotalDays.ToString("F2", CultureInfo.InvariantCulture) + "d";
            }
        }
    }
}
