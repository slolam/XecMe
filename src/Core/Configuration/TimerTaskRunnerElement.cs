using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Core.Tasks;
using System.Configuration;
using System.Diagnostics;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Configuration
{
    /// <summary>
    ///  Type to read the timer task configurations
    /// </summary>
    public class TimerTaskRunnerElement : TaskRunnerElement
    {
        #region Constants
        private static readonly TimeSpan TS_MIN;
        private static readonly TimeSpan TS_MAX;
        private const string INTERVAL = "interval";
        private const string RECURRENCE = "recurrence";
        private const string START = "startDateTime";
        private const string END = "endDateTime";
        private const string DAY_START_TIME = "dayStartTime";
        private const string DAY_END_TIME = "dayEndTime";
        private const string TIME_ZONE = "timeZone";
        private const string WEEK_DAYS = "weekdays";
        #endregion

        /// <summary>
        /// Type initializer for the timer task element
        /// </summary>
        static TimerTaskRunnerElement()
        {
            TS_MIN = TimeSpan.FromSeconds(0);
            TS_MAX = TimeSpan.FromSeconds(86400);
        }

        /// <summary>
        /// Constructor to create this type of instance
        /// </summary>
        public TimerTaskRunnerElement()
        {
            base[START] = DateTime.MinValue;
            base[END] = DateTime.MaxValue;
            base[DAY_START_TIME] = TimeSpan.FromSeconds(0.0);//Midnight
            base[DAY_END_TIME] = TimeSpan.FromSeconds(86399.0);//23:59:59
        }

        /// <summary>
        /// Gets or sets the valid weekdays for this instance
        /// </summary>
        [ConfigurationProperty(WEEK_DAYS, IsRequired = false, DefaultValue=Weekdays.All)]
        public Weekdays Weekdays
        {
            get { return (Weekdays)base[WEEK_DAYS]; }
            set { base[WEEK_DAYS] = value; }
        }

        /// <summary>
        /// Gets or sets the interval of the instance
        /// </summary>
        [ConfigurationProperty(INTERVAL, IsRequired = true)]
        public long Interval
        {
            get { return (long)base[INTERVAL]; }
            set { base[INTERVAL] = value; }
        }

        /// <summary>
        /// Gets or sets the reccurrence of the instance. Default value is -1 that indicates there is no upper bound defined for this task
        /// </summary>
        [ConfigurationProperty(RECURRENCE, IsRequired = false, DefaultValue = -1L)]
        [LongValidator(MinValue = -1L, MaxValue = long.MaxValue)]
        public long Recurrence
        {
            get { return (long)base[RECURRENCE]; }
            set { base[RECURRENCE] = value; }
        }

        /// <summary>
        /// Gets or sets the start date time for this instance
        /// </summary>
        [ConfigurationProperty(START, IsRequired = false)]
        public DateTime StartDateTime
        {
            get { return (DateTime)base[START]; }
            set
            {
                if (EndDateTime < value)
                    throw new ConfigurationErrorsException(string.Concat(START, " should be less than ", END));

                base[START] = value;
            }
        }

        /// <summary>
        /// Gets or sets the end date time for this instance
        /// </summary>
        [ConfigurationProperty(END, IsRequired = false)]
        public DateTime EndDateTime
        {
            get { return (DateTime)base[END]; }
            set
            {
                if (StartDateTime > value)
                    throw new ConfigurationErrorsException(string.Concat(END, " should be greater than ", START));

                base[END] = value;
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
                catch(Exception e)
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
        /// Gets or sets the start time for this instance
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
        /// Gets or sets the end time for this instance
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


        /// <summary>
        /// Returns the instance of TimerTaskRunner type
        /// </summary>
        /// <returns></returns>
        public override TaskRunner GetRunner()
        {
            TimeZoneInfo tz = null;
            string tzn = TimeZoneName;
            if (!string.IsNullOrEmpty(tzn))
                tz = TimeZoneInfo.FindSystemTimeZoneById(tzn);
            return new TimerTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), Interval, Recurrence, Weekdays,
                    this.StartDateTime, this.EndDateTime, this.DayStartTime, this.DayEndTime, tz, TraceFilter);
        }
    }
}
