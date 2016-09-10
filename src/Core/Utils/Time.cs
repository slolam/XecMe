using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;
using Util = XecMe.Core.Tasks.Utils;

namespace XecMe.Core.Utils
{
    /// <summary>
    /// Time calculations functions
    /// </summary>
    static class Time
    {
        /// <summary>
        /// The day minimum time mid night 12:00 AM
        /// </summary>
        public static readonly TimeSpan DayMinTime;
        /// <summary>
        /// The day maximum time almost mid night 11:59:59 PM
        /// </summary>
        public static readonly TimeSpan DayMaxTime;
        static Time()
        {
            DayMinTime = TimeSpan.FromSeconds(0);
            DayMaxTime = TimeSpan.FromSeconds(86399);
        }

        /// <summary>
        /// Disallows the specified today.
        /// </summary>
        /// <param name="today">The today.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="weekdays">The weekdays.</param>
        /// <returns>Returns true if execution should be allowed else false</returns>
        internal static bool Disallow(DateTime today, TimeSpan startTime, TimeSpan endTime, Weekdays weekdays)
        {
            TimeSpan now = today.TimeOfDay;
                    ///Out of the time range with start and time on the same day
            return ((now < startTime|| endTime < now) && startTime < endTime)
                    ///Out of time range and job time spanning across the days
                    || ((endTime < now && now < startTime) && endTime < startTime)
                    ///Not a valid weekday
                    || !Util.HasWeekday(weekdays, Util.GetWeekday(today.DayOfWeek));
        }
    }
}
