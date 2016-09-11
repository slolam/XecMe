using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using XecMe.Core.Tasks;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Configuration
{
    /// <summary>
    /// Type to read the scheduled task configurations
    /// </summary>
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

        /// <summary>
        /// Type initializer setting the constants
        /// </summary>
        static ScheduledTaskRunnerElement()
        {
            TS_MIN = TimeSpan.FromSeconds(0.0);
            TS_MAX = TimeSpan.FromSeconds(86399.0);
        }

        /// <summary>
        /// Constructor to create the new instance of tyis type
        /// </summary>
        public ScheduledTaskRunnerElement()
        {
            base[START] = DateTime.MinValue;
            base[TASKTIME] = TimeSpan.FromSeconds(0);
            base[SCHEDULE] = string.Empty;
        }

        /// <summary>
        /// Gets or sets recursion for this configuration instance
        /// </summary>
        [ConfigurationProperty(RECURSION, IsRequired = true)]
        public Recursion Recursion
        {
            get { return (Recursion)base[RECURSION]; }
            set { base[RECURSION] = value; }
        }

        /// <summary>
        /// Gets or sets the schedule string for this configuration instance
        /// </summary>
        [ConfigurationProperty(SCHEDULE, IsRequired = false)]
        public string Schedule
        {
            get { return (string)base[SCHEDULE]; }
            set { base[SCHEDULE] = value; }
        }

        /// <summary>
        /// Gets or sets the repeat  for this configuration instance that defines how often to repeat, the default is 1 indicating every occurrence
        /// </summary>
        [ConfigurationProperty(REPEAT, IsRequired = false, DefaultValue = 1)]
        [IntegerValidator(MinValue = 1, MaxValue = short.MaxValue)]
        public int Repeat
        {
            get { return (int)base[REPEAT]; }
            set { base[REPEAT] = value; }
        }

        /// <summary>
        /// Gets or sets start date for this configuration instance
        /// </summary>
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
                    Log.Error(string.Format("Error reading Scheduled Task: {0}", e));
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
        /// Gets or set the task time for this configuration instance.
        /// </summary>
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


        /// <summary>
        /// Returns the instance of the ScheduledTaskRunner type
        /// </summary>
        /// <returns>Returns the ScheduledTaskRunner instance</returns>
        public override TaskRunner GetRunner()
        {
            TimeZoneInfo tz = null;
            string tzn = TimeZoneName;
            if (!string.IsNullOrEmpty(tzn))
                tz = TimeZoneInfo.FindSystemTimeZoneById(tzn);
            return new ScheduledTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), (uint)Repeat, Recursion,
                    this.Schedule, this.StartDate, this.TaskTime, tz, TraceFilter);
        }
    }
}
