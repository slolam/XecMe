using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent.Scheduled
{
    /// <summary>
    /// Interface to define Schedulable tasks
    /// </summary>
    public interface IScheduledTaskBuilder
    {
        /// <summary>
        /// Schedule a task to run it daily
        /// </summary>
        /// <returns>Returns <see cref="IScheduledRepeat"/></returns>
        IScheduledRepeat RunDaily();

        /// <summary>
        /// Schedule a task to run it weekly
        /// </summary>
        /// <returns>Returns <see cref="IScheduledWeekdays"/></returns>
        IScheduledWeekdays RunWeekly();

        /// <summary>
        /// Schedule a task to run it by days of specific months
        /// </summary>
        /// <param name="months">Months the task is scheduled for</param>
        /// <returns>Returns <see cref="IScheduledDays"/></returns>
        IScheduledDays RunByDaysOfTheMonths(Months months);

        /// <summary>
        /// Schedule a task to run it by weeks of specified months
        /// </summary>
        /// <param name="months">Months the task is scheduled for</param>
        /// <returns>Returns <see cref="IScheduledWeeks"/></returns>
        IScheduledWeeks RunByWeeksOfTheMonths(Months months);

        //IScheduledMonths Monthly();
    }

    /// <summary>
    /// Interface to define the start date time
    /// </summary>
    public interface IScheduledPeriod
    {
        /// <summary>
        /// Start date time from when the task schedule to be calculated for skips
        /// </summary>
        /// <param name="start">Date time from when the task to be calculated for skips</param>
        /// <returns>Returns <see cref="IScheduledExecuteAt"/></returns>
        IScheduledExecuteAt StartsFrom(DateTime start);

        /// <summary>
        /// Start scheduling immediately
        /// </summary>
        /// <returns>Returns <see cref="IScheduledExecuteAt"/></returns>
        IScheduledExecuteAt StartsNow();
    }

    /// <summary>
    /// Interface to define the timezone of the task time.
    /// </summary>
    public interface IScheduledTimeZone
    {
        /// <summary>
        /// Schedule the task for the local timezone of the server
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
    /// Interface to define the task execution time
    /// </summary>
    public interface IScheduledExecuteAt
    {
        /// <summary>
        /// Specific time of the day when the task to run
        /// </summary>
        /// <param name="taskTime">Time of the day</param>
        /// <returns>Returns <see cref="IScheduledTimeZone"/></returns>
        IScheduledTimeZone RunsAt(TimeSpan taskTime);
    }

    /// <summary>
    /// Interface to define number of skip between the execution
    /// </summary>
    public interface IScheduledRepeat
    {
        /// <summary>
        /// Repeat every other weeks/days/months. Start date time is needed for this calculations
        /// </summary>
        /// <param name="every">Skip number of times</param>
        /// <returns>Returns <see cref="IScheduledPeriod"/></returns>
        IScheduledPeriod RepeatEvery(uint every);
    }


    /// <summary>
    /// Interface to define the weekdays for executing the task
    /// </summary>
    public interface IScheduledWeekdays
    {
        /// <summary>
        /// Specific weekdays when the task to be run
        /// </summary>
        /// <param name="weekdays">Weekdays when the task to be run</param>
        /// <returns>Returns <see cref="IScheduledRepeat"/></returns>
        IScheduledRepeat OnWeekdays(Weekdays weekdays);
    }

    /// <summary>
    /// Interface to define the weekdays for executing the task
    /// </summary>
    public interface IScheduledMonthlyWeekdays
    {
        /// <summary>
        /// Specific weekdays when the task to be run
        /// </summary>
        /// <param name="weekdays">Weekdays when the task to be run</param>
        /// <returns>Returns <see cref="IScheduledExecuteAt"/></returns>
        IScheduledExecuteAt OnWeekdays(Weekdays weekdays);
    }

    /// <summary>
    /// Interface to define the days on which the task executes
    /// </summary>
    public interface IScheduledDays
    {
        /// <summary>
        /// Run the task on specific days
        /// </summary>
        /// <param name="days">Days of the month when the task to be executed</param>
        /// <returns>Returns <see cref="IScheduledExecuteAt"/></returns>
        IScheduledExecuteAt OnDays(params uint[] days);
    }

    /// <summary>
    /// Interface to define the weeks in which task should run
    /// </summary>
    public interface IScheduledWeeks
    {
        /// <summary>
        /// Weeks when the task to be scheduled
        /// </summary>
        /// <param name="weeks">Week of the months</param>
        /// <returns>Returns <see cref="IScheduledMonthlyWeekdays"/></returns>
        IScheduledMonthlyWeekdays OnWeeks(Weeks weeks);
    }


    /// <summary>
    /// Scheduled task builder
    /// </summary>
    /// <seealso cref="TaskBuilder" />
    /// <seealso cref="IScheduledTaskBuilder" />
    /// <seealso cref="IScheduledDays" />
    /// <seealso cref="IScheduledExecuteAt" />
    /// <seealso cref="IScheduledTimeZone" />
    /// <seealso cref="IScheduledPeriod" />
    /// <seealso cref="IScheduledRepeat" />
    /// <seealso cref="IScheduledWeekdays" />
    /// <seealso cref="IScheduledWeeks" />
    /// <seealso cref="IScheduledMonthlyWeekdays" />
    internal class ScheduledTaskBuilder: TaskBuilder, IScheduledTaskBuilder, IScheduledDays, IScheduledExecuteAt, 
        IScheduledTimeZone, IScheduledPeriod, IScheduledRepeat, IScheduledWeekdays, IScheduledWeeks, IScheduledMonthlyWeekdays
    {
        Recursion _recursion;
        DateTime _startDate;
        TimeSpan _taskTime;
        uint _repeat;
        uint _days = 0;
        TimeZoneInfo _timeZone;
        Weeks _weeks = Tasks.Weeks.None;
        Months _months = Tasks.Months.None;
        Weekdays _weekdays = Tasks.Weekdays.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskBuilder"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="logType">Type of the log.</param>
        internal ScheduledTaskBuilder(FlowConfiguration config, string name, Type taskType, LogType logType)
            : base(config, name, taskType, logType) { }

        /// <summary>
        /// Add the task to the configuration
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// Missing the attributes for the Monthly task
        /// or
        /// Recursion is not set
        /// </exception>
        public override void Add()
        {
            IRecur recur = null;
            
            switch (_recursion)
            {
                case Recursion.Daily:
                    recur = new Daily(_repeat);
                    break;
                case Recursion.Weekly:
                    recur = new Weekly(_repeat, _weekdays);
                    break;
                case Recursion.Monthly:
                    if (_weeks != Tasks.Weeks.None && _weekdays != Tasks.Weekdays.None && _months != Tasks.Months.None)
                        recur = new MonthlyByWeekdays(_months, _weekdays, _weeks);
                    else if (_days >= 1 && _months != Tasks.Months.None)
                        recur = new MonthlyByDay(_months, _days);
                    else
                        throw new InvalidOperationException($"Missing the attributes for the Monthly task");
                    break;
                default:
                    throw new InvalidOperationException("Recursion is not set");
            }

            Config.InternalRunners.Add(new ScheduledTaskRunner(Name, TaskType, Parameters, recur, _startDate, _taskTime, _timeZone, TraceType));
        }

        /// <summary>
        /// Start date time from when the task schedule to be calculated for skips
        /// </summary>
        /// <param name="start">Date time from when the task to be calculated for skips</param>
        /// <returns>
        /// Returns <see cref="IScheduledExecuteAt" />
        /// </returns>
        IScheduledExecuteAt IScheduledPeriod.StartsFrom(DateTime start)
        {
            _startDate = start;
            return this;
        }

        /// <summary>
        /// Start scheduling immediately
        /// </summary>
        /// <returns>
        /// Returns <see cref="IScheduledExecuteAt" />
        /// </returns>
        IScheduledExecuteAt IScheduledPeriod.StartsNow()
        {
            _startDate = DateTime.Now;
            return this;
        }

        /// <summary>
        /// Schedule the task for the local timezone of the server
        /// </summary>
        /// <returns>
        /// Returns <see cref="ITaskBuilder" />
        /// </returns>
        ITaskBuilder IScheduledTimeZone.OfLocalTimeZone()
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
        ITaskBuilder IScheduledTimeZone.OfTimeZone(string timeZoneId)
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
        ITaskBuilder IScheduledTimeZone.OfUtcTimeZone()
        {
            _timeZone = TimeZoneInfo.Utc;
            return this;
        }

        /// <summary>
        /// Specific time of the day when the task to run
        /// </summary>
        /// <param name="taskTime">Time of the day</param>
        /// <returns>
        /// Returns <see cref="IScheduledTimeZone" />
        /// </returns>
        IScheduledTimeZone IScheduledExecuteAt.RunsAt(TimeSpan taskTime)
        {
            _taskTime = taskTime;
            return this;
        }

        /// <summary>
        /// Schedule a task to run it daily
        /// </summary>
        /// <returns>
        /// Returns <see cref="IScheduledRepeat" />
        /// </returns>
        IScheduledRepeat IScheduledTaskBuilder.RunDaily()
        {
            _recursion = Recursion.Daily;
            return this;
        }

        /// <summary>
        /// Schedule a task to run it weekly
        /// </summary>
        /// <returns>
        /// Returns <see cref="IScheduledWeekdays" />
        /// </returns>
        IScheduledWeekdays IScheduledTaskBuilder.RunWeekly()
        {
            _recursion = Recursion.Weekly;
            return this;
        }

        /// <summary>
        /// Run the task on specific days
        /// </summary>
        /// <param name="days">Days of the month when the task to be executed</param>
        /// <returns>
        /// Returns <see cref="IScheduledExecuteAt" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        IScheduledExecuteAt IScheduledDays.OnDays(params uint[] days)
        {
            for (int i =0; i < days.Length; i++)
            {
                uint day = days[i];
                //For all
                if(day == (uint)Days.All)
                {
                    _days = day;
                }
                else if(day == (uint)Days.Last)
                {
                    _days = day;
                }
                else
                {
                    if(day == 0 || day > 31)
                    {
                        throw new ArgumentOutOfRangeException(nameof(days));
                    }
                    uint d = 1;
                    _days |= (d << (int)--day);
                }
            }
            return this;
        }

        /// <summary>
        /// Repeat every other weeks/days/months. Start date time is needed for this calculations
        /// </summary>
        /// <param name="every">Skip number of times</param>
        /// <returns>
        /// Returns <see cref="IScheduledPeriod" />
        /// </returns>
        IScheduledPeriod IScheduledRepeat.RepeatEvery(uint every)
        {
            _repeat = every;
            return this;
        }


        /// <summary>
        /// Weeks when the task to be scheduled
        /// </summary>
        /// <param name="weeks">Week of the months</param>
        /// <returns>
        /// Returns <see cref="IScheduledMonthlyWeekdays" />
        /// </returns>
        IScheduledMonthlyWeekdays IScheduledWeeks.OnWeeks(Weeks weeks)
        {
            _weeks = weeks;
            return this;
        }

        /// <summary>
        /// Schedule a task to run it by days of specific months
        /// </summary>
        /// <param name="months">Months the task is scheduled for</param>
        /// <returns>
        /// Returns <see cref="IScheduledDays" />
        /// </returns>
        IScheduledDays IScheduledTaskBuilder.RunByDaysOfTheMonths(Months months)
        {
            _months = months;
            _recursion = Recursion.Monthly;
            return this;
        }

        /// <summary>
        /// Schedule a task to run it by weeks of specified months
        /// </summary>
        /// <param name="months">Months the task is scheduled for</param>
        /// <returns>
        /// Returns <see cref="IScheduledWeeks" />
        /// </returns>
        IScheduledWeeks IScheduledTaskBuilder.RunByWeeksOfTheMonths(Months months)
        {
            _months = months;
            _recursion = Recursion.Monthly;
            return this;
        }

        /// <summary>
        /// Specific weekdays when the task to be run
        /// </summary>
        /// <param name="weekdays">Weekdays when the task to be run</param>
        /// <returns>
        /// Returns <see cref="IScheduledRepeat" />
        /// </returns>
        IScheduledRepeat IScheduledWeekdays.OnWeekdays(Weekdays weekdays)
        {
            _weekdays = weekdays;
            return this;
        }

        /// <summary>
        /// Specific weekdays when the task to be run
        /// </summary>
        /// <param name="weekdays">Weekdays when the task to be run</param>
        /// <returns>
        /// Returns <see cref="IScheduledExecuteAt" />
        /// </returns>
        IScheduledExecuteAt IScheduledMonthlyWeekdays.OnWeekdays(Weekdays weekdays)
        {
            _weekdays = weekdays;
            return this;
        }
    }
}
