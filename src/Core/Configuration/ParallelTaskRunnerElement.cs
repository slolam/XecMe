#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ParallelTaskRunnerElement.cs is part of XecMe.
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
using XecMe.Core.Tasks;
using XecMe.Core.Utils;
using System.Configuration;
using System.Diagnostics;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Configuration
{
    /// <summary>
    /// Type to read the Task Runner configuration
    /// </summary>
    public class ParallelTaskRunnerElement: TaskRunnerElement
    {
        #region Constants
        private static readonly TimeSpan TS_MIN;
        private static readonly TimeSpan TS_MAX;
        private const string MIN_INSTANCE = "minInstances";
        private const string MAX_INSTANCE = "maxInstances";
        private const string IDLE_POLLING_PERIOD = "idlePollingPeriod";
        //private const string SINGLETON = "singleton";
        private const string WEEK_DAYS = "weekdays";
        private const string DAY_START_TIME = "dayStartTime";
        private const string DAY_END_TIME = "dayEndTime";
        private const string TIME_ZONE = "timeZone";
        #endregion

        /// <summary>
        /// Type initializer PArallel task runner
        /// </summary>
        static ParallelTaskRunnerElement()
        {
            TS_MIN = TimeSpan.FromSeconds(0);
            TS_MAX = TimeSpan.FromSeconds(86400);
        }

        /// <summary>
        /// Constructor to create the instance of the parallel task runner
        /// </summary>
        public ParallelTaskRunnerElement()
        {
            base[DAY_START_TIME] = TimeSpan.FromSeconds(0.0);//Midnight
            base[DAY_END_TIME] = TimeSpan.FromSeconds(86399.0);//23:59:59
        }

        /// <summary>
        /// Gets or sets the idle polling period when there is not task to execute
        /// </summary>
        [ConfigurationProperty(IDLE_POLLING_PERIOD, DefaultValue=300000L), LongValidator(MinValue=100)]
        public long IdlePollingPeriod
        {
            get { return (long)base[IDLE_POLLING_PERIOD]; }
            set { base[IDLE_POLLING_PERIOD] = value; }
        }

        /// <summary>
        /// Gets or sets the weekdays allowed to run the parallel task
        /// </summary>
        [ConfigurationProperty(WEEK_DAYS, IsRequired = false, DefaultValue = Weekdays.All)]
        public Weekdays Weekdays
        {
            get { return (Weekdays)base[WEEK_DAYS]; }
            set { base[WEEK_DAYS] = value; }
        }

        /// <summary>
        /// Gets or set the time zone for this configuration instance. It should be a valid time zone id string from the list TimeZoneInfo.GetSystemTimeZones()
        /// </summary>
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
                    Log.Error(string.Format("Error reading Timer Task: {0}", e));
                    Log.Error("Valid timeZones are ....");
                    foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
                    {
                        Log.Error(string.Format("{0} - {1}", tz.Id, tz.StandardName));
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets or sets the start time during the day after which the task can task
        /// </summary>
        [ConfigurationProperty(DAY_START_TIME, IsRequired = false)]
        public TimeSpan DayStartTime
        {
            get { return (TimeSpan)base[DAY_START_TIME]; }
            set
            {
                if (TS_MIN > value)
                    throw new ConfigurationErrorsException(string.Concat(DAY_START_TIME, " should be greater than 0 seconds"));

                //if (DayEndTime < value)
                //    throw new ConfigurationErrorsException(string.Concat(DAY_START_TIME, " is greater than ", DAY_END_TIME));

                base[DAY_START_TIME] = value;
            }
        }

        /// <summary>
        /// Gets or sets the end time of the task after which the task stop to run
        /// </summary>
        [ConfigurationProperty(DAY_END_TIME, IsRequired = false)]
        public TimeSpan DayEndTime
        {
            get { return (TimeSpan)base[DAY_END_TIME]; }
            set
            {
                if (TS_MAX < value)
                    throw new ConfigurationErrorsException(string.Concat(DAY_END_TIME, " should be less than 23:59:59"));

                //if (DayStartTime > value)
                //    throw new ConfigurationErrorsException(string.Concat(DAY_END_TIME, " is less than ", DAY_START_TIME));

                base[DAY_END_TIME] = value;
            }
        }

        //[ConfigurationProperty(SINGLETON, DefaultValue = false)]
        //public bool Singleton
        //{
        //    get { return (bool)base[SINGLETON]; }
        //    set { base[SINGLETON] = value; }
        //}

        /// <summary>
        /// Gets or sets the minimum number threads that should be reserved for this task. 
        /// </summary>
        [ConfigurationProperty(MIN_INSTANCE, IsRequired = true, DefaultValue = 1), IntegerValidator(MinValue = 1)]
        public int MinInstances
        {
            get { return (int)base[MIN_INSTANCE]; }
            set { base[MIN_INSTANCE] = value; }
        }


        /// <summary>
        /// Gets or sets the maximum number of threads that this instance will run on
        /// </summary>
        [ConfigurationProperty(MAX_INSTANCE, IsRequired = true, DefaultValue=1), IntegerValidator(MinValue=1)]
        public int MaxInstances
        {
            get { return (int)base[MAX_INSTANCE]; }
            set { base[MAX_INSTANCE] = value; }
        }

        /// <summary>
        /// Returns the instance of the TaskRunner 
        /// </summary>
        /// <returns>Instance of the ParallelTaskRunner</returns>
        public override TaskRunner GetRunner()
        {
            TimeZoneInfo tz = null;
            string tzn = TimeZoneName;
            if (!string.IsNullOrEmpty(tzn))
                tz = TimeZoneInfo.FindSystemTimeZoneById(tzn);
            return new ParallelTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), MinInstances, MaxInstances, 
                IdlePollingPeriod, DayStartTime, DayEndTime, Weekdays, tz, TraceFilter);
        }

    }
}
