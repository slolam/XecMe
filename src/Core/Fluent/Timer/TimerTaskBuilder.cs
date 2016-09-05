using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;
using XecMe.Core.Tasks;
using XecMe.Core.Utils;

namespace XecMe.Core.Fluent.Timer
{
    /// <summary>
    /// Interface to define the milliseconds interval for the timer job 
    /// </summary>
    public interface ITimerTaskBuilder
    {
        /// <summary>
        /// Runs the task every x milliseconds
        /// </summary>
        /// <param name="milliSeconds">The milliseconds.</param>
        /// <returns>Returns <see cref="IRepeat"/></returns>
        IRepeat RunEvery(long milliSeconds);
    }

    /// <summary>
    /// Interface to define the number of times the timer task to be run
    /// </summary>
    public interface IRepeat
    {
        /// <summary>
        /// Limit the number of times to execution 
        /// </summary>
        /// <param name="times">The number of times</param>
        /// <returns>Return <see cref="IWeekdays"/></returns>
        /// <remarks>Use -1 for no limit</remarks>
        IWeekdays RepeatFor(long times);
    }

    /// <summary>
    /// Interface to define the life time of the task
    /// </summary>
    public interface ILifetime
    {
        /// <summary>
        /// Execute task durings this period.
        /// </summary>
        /// <param name="from">From this date</param>
        /// <param name="to">To this date</param>
        /// <returns>Returns <see cref="IOperationalTime"/></returns>
        IOperationalTime DuringPeriod(DateTime startDateTime, DateTime endDateTime);
    }

    /// <summary>
    /// Interface to define the operation time of the task during the day
    /// </summary>
    public interface IOperationalTime
    {
        /// <summary>
        /// The task run durings this time of day.
        /// </summary>
        /// <param name="from">From time of the day</param>
        /// <param name="to">To time of the day</param>
        /// <returns>Returns <see cref="ITimeZone"/></returns>
        ITimeZone BetweenTimeOfDay(TimeSpan from, TimeSpan to);
    }

    /// <summary>
    /// Interface to define the weekdays when the task should run
    /// </summary>
    public interface IWeekdays
    {
        /// <summary>
        /// Runs the on weekdays.
        /// </summary>
        /// <param name="weekdays">The weekdays.</param>
        /// <returns>Returns <see cref="ILifetime"/></returns>
        ILifetime OnWeekdays(Weekdays weekdays);
    }


    /// <summary>
    /// Interfacce to define the timezone for the task time calculations
    /// </summary>
    public interface ITimeZone
    {
        /// <summary>
        /// Schedule the task for the local timezone
        /// </summary>
        /// <returns>Returns <see cref="ITaskBuilder"/></returns>
        ITaskBuilder OfLocalTimeZone();

        /// <summary>
        /// Specific timezone
        /// </summary>
        /// <param name="timeZoneId">Timezone id</param>
        /// <returns>Returns <see cref="ITaskBuilder"/></returns>
        ITaskBuilder OfTimeZone(string timeZoneId);

        /// <summary>
        /// Time calculation will be based on UTC timezone
        /// </summary>
        /// <returns>Returns <see cref="ITaskBuilder"/></returns>
        ITaskBuilder OfUtcTimeZone();
    }

    /// <summary>
    /// Timer task builder
    /// </summary>
    /// <seealso cref="XecMe.Core.Fluent.TaskBuilder" />
    /// <seealso cref="XecMe.Core.Fluent.Timer.ITimerTaskBuilder" />
    /// <seealso cref="XecMe.Core.Fluent.Timer.ILifetime" />
    /// <seealso cref="XecMe.Core.Fluent.Timer.IRepeat" />
    /// <seealso cref="XecMe.Core.Fluent.Timer.ITimeZone" />
    /// <seealso cref="XecMe.Core.Fluent.Timer.IWeekdays" />
    /// <seealso cref="XecMe.Core.Fluent.Timer.IOperationalTime" />
    internal class TimerTaskBuilder: TaskBuilder, ITimerTaskBuilder, ILifetime, IRepeat, ITimeZone, IWeekdays, IOperationalTime
    {
        private long _interval;
        private long _recurrence;
        private DateTime _startDateTime = DateTime.MinValue;
        private DateTime _endDateTime = DateTime.MaxValue;
        private TimeSpan _dayStartTime = TimeSpan.FromSeconds(0);
        private TimeSpan _dayEndTime = TimeSpan.FromSeconds(86399);
        private Weekdays _weekdays = Weekdays.All;
        private TimeZoneInfo _timeZone = TimeZoneInfo.Local;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerTaskBuilder"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="logType">Type of the log.</param>
        internal TimerTaskBuilder(FlowConfiguration config, string name, Type taskType, LogType logType = LogType.All)
            : base(config, name, taskType, logType) { }

        /// <summary>
        /// Add the task to the configuration
        /// </summary>
        public override void Add()
        {
            Config.InternalRunners.Add(new TimerTaskRunner(Name, TaskType, Parameters, _interval, _recurrence, _weekdays, _startDateTime, _endDateTime, _dayStartTime, _dayEndTime, _timeZone, TraceType));
        }

        /// <summary>
        /// Runs the task every x milliseconds
        /// </summary>
        /// <param name="milliSeconds">The milliseconds.</param>
        /// <returns>
        /// Returns <see cref="IRepeat" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">{nameof(milliSeconds)}</exception>
        public IRepeat RunEvery(long milliSeconds)
        {
            if (milliSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(milliSeconds), $"{nameof(milliSeconds)} cannot be negative");
            _interval = milliSeconds;
            return this;
        }

        /// <summary>
        /// Execute task durings this period.
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <returns>
        /// Returns <see cref="IOperationalTime" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">{nameof(startDateTime)} should be less than {nameof(endDateTime)}")</exception>
        public IOperationalTime DuringPeriod(DateTime startDateTime, DateTime endDateTime)
        {
            if (endDateTime < startDateTime)
                throw new ArgumentOutOfRangeException(nameof(startDateTime), $"{nameof(startDateTime)} should be less than {nameof(endDateTime)}");
            _startDateTime = startDateTime;
            _endDateTime = endDateTime;
            return this;
        }

        /// <summary>
        /// Limit the number of times to execution
        /// </summary>
        /// <param name="times">The number of times</param>
        /// <returns>
        /// Return <see cref="IWeekdays" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Number of times to be repeated should be greater than 0 or euqal to -1</exception>
        /// <remarks>
        /// Use -1 for no limit
        /// </remarks>
        IWeekdays IRepeat.RepeatFor(long times)
        {
            if (times != -1 && times < 1)
                throw new ArgumentOutOfRangeException(nameof(times), "Number of times to be repeated should be greater than 0 or euqal to -1");
            _recurrence = times;
            return this;
        }

        /// <summary>
        /// Schedule the task for the local timezone
        /// </summary>
        /// <returns>
        /// Returns <see cref="ITaskBuilder" />
        /// </returns>
        ITaskBuilder ITimeZone.OfLocalTimeZone()
        {
            _timeZone = TimeZoneInfo.Local;
            return this;
        }

        /// <summary>
        /// Specific timezone
        /// </summary>
        /// <param name="timeZoneId">Timezone id</param>
        /// <returns>
        /// Returns <see cref="ITaskBuilder" />
        /// </returns>
        ITaskBuilder ITimeZone.OfTimeZone(string timeZoneId)
        {
            timeZoneId.NotNullOrWhiteSpace(nameof(timeZoneId));
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return this;
        }

        /// <summary>
        /// Time calculation will be based on UTC timezone
        /// </summary>
        /// <returns>
        /// Returns <see cref="ITaskBuilder" />
        /// </returns>
        ITaskBuilder ITimeZone.OfUtcTimeZone()
        {
            _timeZone = TimeZoneInfo.Utc;
            return this;
        }

        /// <summary>
        /// Runs the on weekdays.
        /// </summary>
        /// <param name="weekdays">The weekdays.</param>
        /// <returns>
        /// Returns <see cref="ILifetime" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">No weekday is selected to run the task</exception>
        ILifetime IWeekdays.OnWeekdays(Weekdays weekdays)
        {
            if(weekdays == Weekdays.None)
                throw new ArgumentOutOfRangeException(nameof(weekdays), "No weekday is selected to run the task");
            _weekdays = weekdays;
            return this;
        }

        /// <summary>
        /// Betweens the time of day.
        /// </summary>
        /// <param name="dayStartTime">The day start time.</param>
        /// <param name="dayEndTime">The day end time.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Start time cannot be negative
        /// or
        /// End time cannot be negative
        /// or
        /// Start time should be less than 23:59:59
        /// or
        /// End time should be less than 23:59:59
        /// </exception>
        ITimeZone IOperationalTime.BetweenTimeOfDay(TimeSpan dayStartTime, TimeSpan dayEndTime)
        {
            if (dayStartTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException(nameof(dayStartTime), "Start time cannot be negative");

            if (dayEndTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException(nameof(dayEndTime), "End time cannot be negative");

            if (dayStartTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(dayStartTime), "Start time should be less than 23:59:59");

            if (dayEndTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(dayEndTime), "End time should be less than 23:59:59");

            _dayStartTime = dayStartTime;
            _dayEndTime = dayEndTime;
            return this;
        }
    }
}
