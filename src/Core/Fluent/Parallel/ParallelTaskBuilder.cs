using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;
using XecMe.Core.Tasks;
using XecMe.Core.Utils;

namespace XecMe.Core.Fluent.Parallel
{
    /// <summary>
    /// Interface to define minimum number of instances
    /// </summary>
    public interface IParallelTaskBuilder
    {
        /// <summary>
        /// With the minimum number of instances of task
        /// </summary>
        /// <param name="min">Minimum number of task instances</param>
        /// <returns>Returns <see cref="IMaximumInstances"/></returns>
        IMaximumInstances RunWithMinimumInstancesOf(uint min);
    }


    /// <summary>
    /// Interface to define maximum number of instances
    /// </summary>
    public interface IMaximumInstances
    {
        /// <summary>
        /// The maximum number of instances of task
        /// </summary>
        /// <param name="min">Maxinum number of task instances</param>
        /// <returns>Returns <see cref="IIdlePeriod"/> </returns>
        IIdlePeriod AndMaximumInstancesOf(uint max);
    }


    /// <summary>
    /// Interface to define the operational time
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
    /// Interface to define the idle period when no work
    /// </summary>
    public interface IIdlePeriod
    {
        /// <summary>
        /// Idle polling period when there is nothing to process
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns>Returns <see cref="IWeekdays"/></returns>
        IWeekdays WithIdlePeriod(ulong period);
    }

    /// <summary>
    /// Interface to define the weekdays when task to run
    /// </summary>
    public interface IWeekdays
    {
        /// <summary>
        /// Runs the on weekdays.
        /// </summary>
        /// <param name="weekdays">The weekdays.</param>
        /// <returns>Returns <see cref="IOperationalTime"/></returns>
        IOperationalTime OnWeekdays(Weekdays weekdays);
    }

    /// <summary>
    /// Interface to define the timezone for the task time
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
    /// Parallel task builder
    /// </summary>
    /// <seealso cref="TaskBuilder" />
    /// <seealso cref="IParallelTaskBuilder" />
    /// <seealso cref="IMaximumInstances" />
    /// <seealso cref="IIdlePeriod" />
    /// <seealso cref="IOperationalTime" />
    /// <seealso cref="IWeekdays" />
    /// <seealso cref="ITimeZone" />
    internal class ParallelTaskBuilder: TaskBuilder, IParallelTaskBuilder, IMaximumInstances, IIdlePeriod, IOperationalTime, IWeekdays, ITimeZone
    {
        uint _min, _max;
        ulong _period;
        TimeSpan _from, _to;
        Weekdays _weekdays;
        TimeZoneInfo _timeZone;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelTaskBuilder"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="logType">Type of the log.</param>
        internal ParallelTaskBuilder(FlowConfiguration config, string name, Type taskType, LogType logType = LogType.All)
            :base(config, name, taskType, logType)
        {
            _min = 1;
            _weekdays = Weekdays.All;
            _timeZone = TimeZoneInfo.Local;
            _from = Time.DayMinTime;
            _to = Time.DayMaxTime;
        }

        /// <summary>
        /// Add the task to the configuration
        /// </summary>
        public override void Add()
        {
            Config.InternalRunners.Add(new ParallelTaskRunner(Name, TaskType, Parameters, _min, _max, _period, _from, _to, _weekdays, _timeZone, TraceType));
        }

        /// <summary>
        /// With the minimum number of instances of task
        /// </summary>
        /// <param name="min">Minimum number of task instances</param>
        /// <returns>
        /// Returns <see cref="IMaximumInstances" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Minimum of 1 instance should be defined</exception>
        IMaximumInstances IParallelTaskBuilder.RunWithMinimumInstancesOf(uint min)
        {
            if(min < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(min), $"Minimum of 1 instance should be defined");
            }
            _min = min;
            return this;
        }

        /// <summary>
        /// The maximum number of instances of task
        /// </summary>
        /// <param name="max"></param>
        /// <returns>
        /// Returns <see cref="IIdlePeriod" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        IIdlePeriod IMaximumInstances.AndMaximumInstancesOf(uint max)
        {
            if(max < _min)
            {
                throw new ArgumentOutOfRangeException(nameof(max));
            }
            _max = max;
            return this;
        }

        /// <summary>
        /// Idle polling period when there is nothing to process
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns>
        /// Returns <see cref="IWeekdays" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        IWeekdays IIdlePeriod.WithIdlePeriod(ulong period)
        {
            if(period < 1000)
            {
                throw new ArgumentOutOfRangeException(nameof(period));
            }
            _period = period;
            return this;
        }

        /// <summary>
        /// The task run durings this time of day.
        /// </summary>
        /// <param name="from">From time of the day</param>
        /// <param name="to">To time of the day</param>
        /// <returns>
        /// Returns <see cref="ITimeZone" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// From time cannot be negative
        /// or
        /// To time cannot be negative
        /// or
        /// From time must be less than 23:59:59
        /// or
        /// To time must be less than 23:59:59
        /// or
        /// From time must be less that To time
        /// </exception>
        ITimeZone IOperationalTime.BetweenTimeOfDay(TimeSpan from, TimeSpan to)
        {
            if(from < Time.DayMinTime)
            {
                throw new ArgumentOutOfRangeException(nameof(from), "From time cannot be negative");
            }
            if (to < Time.DayMinTime)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "To time cannot be negative");
            }
            if (from > Time.DayMaxTime)
            {
                throw new ArgumentOutOfRangeException(nameof(from), "From time must be less than 23:59:59");
            }
            if (to > Time.DayMaxTime)
            {
                throw new ArgumentOutOfRangeException(nameof(to), "To time must be less than 23:59:59");
            }
            if (from >= to)
            {
                throw new ArgumentOutOfRangeException(nameof(from),"From time must be less that To time");
            }
            _from = from;
            _to = to;
            return this;
        }

        /// <summary>
        /// Runs the on weekdays.
        /// </summary>
        /// <param name="weekdays">The weekdays.</param>
        /// <returns>
        /// Returns <see cref="IOperationalTime" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Weekdays must at least one week day</exception>
        IOperationalTime IWeekdays.OnWeekdays(Weekdays weekdays)
        {
            if(weekdays == Weekdays.None)
            {
                throw new ArgumentOutOfRangeException(nameof(weekdays),"Weekdays must at least one week day");
            }
            _weekdays = weekdays;
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
            _timeZone = TimeZoneInfo.Local;
            return this;
        }
    }
}
