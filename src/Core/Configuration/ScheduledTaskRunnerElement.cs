#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ScheduledTaskRunnerElement.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         01-2013             Shailesh Lolam

#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using XecMe.Core.Tasks;

namespace XecMe.Core.Configuration
{
    public class ScheduledTaskRunnerElement : TaskRunnerElement
    {
        #region Constants
        private static readonly TimeSpan TS_MIN;
        private static readonly TimeSpan TS_MAX;
        private const string REPEAT = "repeat";
        private const string RECURSION = "recursion";
        private const string START = "startDate";
        private const string TASKTIME = "taskTime";
        private const string SCHEDULE = "schedule";
        private const string TIME_ZONE = "timeZone";
        #endregion

        static ScheduledTaskRunnerElement()
        {
            TS_MIN = TimeSpan.FromSeconds(0.0);
            TS_MAX = TimeSpan.FromSeconds(86399.0);
        }

        public ScheduledTaskRunnerElement()
        {
            base[START] = DateTime.Now.Date;
            base[TASKTIME] = TimeSpan.FromSeconds(0);
            base[SCHEDULE] = string.Empty;
        }

        [ConfigurationProperty(RECURSION, IsRequired = true)]
        public Recursion Recursion
        {
            get { return (Recursion)base[RECURSION]; }
            set { base[RECURSION] = value; }
        }

        [ConfigurationProperty(SCHEDULE, IsRequired = false)]
        public string Schedule
        {
            get { return (string)base[SCHEDULE]; }
            set { base[SCHEDULE] = value; }
        }

        [ConfigurationProperty(REPEAT, IsRequired = false, DefaultValue = 1)]
        [IntegerValidator(MinValue = 1, MaxValue = short.MaxValue)]
        public int Repeat
        {
            get { return (int)base[REPEAT]; }
            set { base[REPEAT] = value; }
        }

        [ConfigurationProperty(START, IsRequired = false)]
        public DateTime StartDate
        {
            get { return (DateTime)base[START]; }
            set
            {
                if (DateTime.Now < value)
                    throw new ConfigurationErrorsException("startDate should be less than today");

                base[START] = value;
            }
        }


        [ConfigurationProperty(TIME_ZONE, IsRequired = false)]
        public string TimeZoneName
        {
            get { return (string)base[TIME_ZONE]; }
            set
            {
                try
                {
                    TimeZoneInfo.FindSystemTimeZoneById(value);
                    base[TIME_ZONE] = value;
                }
                catch (Exception e)
                {
                    Trace.TraceError("Error reading Timer Task: {0}", e);
                    throw;
                }
            }
        }

        [ConfigurationProperty(TASKTIME, IsRequired = true)]
        public TimeSpan TaskTime
        {
            get { return (TimeSpan)base[TASKTIME]; }
            set
            {
                if (TS_MIN < value)
                    throw new ConfigurationErrorsException("taskTime should be greater than 0 seconds");

                if (TS_MAX > value)
                    throw new ConfigurationErrorsException("taskTime should be less than 23:59:59");

                base[TASKTIME] = value;
            }
        }



        public override TaskRunner GetRunner()
        {
            TimeZoneInfo tz = null;
            string tzn = TimeZoneName;
            if (!string.IsNullOrEmpty(tzn))
                tz = TimeZoneInfo.FindSystemTimeZoneById(tzn);
            return new ScheduledTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), Repeat, Recursion,
                    this.Schedule, this.StartDate, this.TaskTime, tz);
        }
    }
}
