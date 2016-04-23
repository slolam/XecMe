using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;
using Util = XecMe.Core.Tasks.Utils;

namespace XecMe.Core.Utils
{
    static class TimeConditions
    {
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
