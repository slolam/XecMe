using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Core.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading;
using System.Diagnostics;
using XecMe.Core.Utils;
using XecMe.Common;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Recursion for the schedule
    /// </summary>
    public enum Recursion
    {
        /// <summary>
        /// The daily
        /// </summary>
        Daily,
        /// <summary>
        /// The weekly
        /// </summary>
        Weekly,
        /// <summary>
        /// The monthly
        /// </summary>
        Monthly
    }


    /// <summary>
    /// Scheduled by days
    /// </summary>
    public enum Days: uint
    {
        /// <summary>
        /// LAst day of the month
        /// </summary>
        Last = 0x80000000,
        /// <summary>
        /// All days of the month
        /// </summary>
        All = 0x7FFFFFFF
    }

    /// <summary>
    /// Weeks of the month
    /// </summary>
    public enum Weeks
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The first week
        /// </summary>
        First = 1,
        /// <summary>
        /// The second week
        /// </summary>
        Second = First * 2,
        /// <summary>
        /// The third week
        /// </summary>
        Third = Second * 2,
        /// <summary>
        /// The fourth week
        /// </summary>
        Fourth = Third * 2,
        /// <summary>
        /// The last week
        /// </summary>
        Last = Fourth * 2,
        /// <summary>
        /// All weeks
        /// </summary>
        All = (Last * 2) - 1
    }

    /// <summary>
    /// Week days
    /// </summary>
    public enum Weekdays
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The Sunday
        /// </summary>
        Sunday = 1,
        /// <summary>
        /// The Monday
        /// </summary>
        Monday = Sunday * 2,
        /// <summary>
        /// The Tuesday
        /// </summary>
        Tuesday = Monday * 2,
        /// <summary>
        /// The Wednesday
        /// </summary>
        Wednesday = Tuesday * 2,
        /// <summary>
        /// The Thursday
        /// </summary>
        Thursday = Wednesday * 2,
        /// <summary>
        /// The Friday
        /// </summary>
        Friday = Thursday * 2,
        /// <summary>
        /// The Saturday
        /// </summary>
        Saturday = Friday * 2,
        /// <summary>
        /// All weekdays
        /// </summary>
        All = (Saturday * 2) - 1
    }

    /// <summary>
    /// The Months
    /// </summary>
    public enum Months : short
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The January
        /// </summary>
        January = 1,
        /// <summary>
        /// The February
        /// </summary>
        February = January * 2,
        /// <summary>
        /// The March
        /// </summary>
        March = February * 2,
        /// <summary>
        /// The April
        /// </summary>
        April = March * 2,
        /// <summary>
        /// The May
        /// </summary>
        May = April * 2,
        /// <summary>
        /// The June
        /// </summary>
        June = May * 2,
        /// <summary>
        /// The July
        /// </summary>
        July = June * 2,
        /// <summary>
        /// The August
        /// </summary>
        August = July * 2,
        /// <summary>
        /// The September
        /// </summary>
        September = August * 2,
        /// <summary>
        /// The October
        /// </summary>
        October = September * 2,
        /// <summary>
        /// The November
        /// </summary>
        November = October * 2,
        /// <summary>
        /// The December
        /// </summary>
        December = November * 2,
        /// <summary>
        /// All months
        /// </summary>
        All = December * 2 - 1
    }

    /// <summary>
    /// Runner that executes the task at scheduled time.
    /// </summary>
    /// <seealso cref="TaskRunner" />
    public class ScheduledTaskRunner : TaskRunner
    {
        /// <summary>
        /// The schedule
        /// </summary>
        private string _schedule;
        /// <summary>
        /// The start date
        /// </summary>
        private DateTime _startDate, _lastDateTime;
        /// <summary>
        /// The task time
        /// </summary>
        private TimeSpan _taskTime;
        /// <summary>
        /// The recursion
        /// </summary>
        private Recursion _recursion;
        /// <summary>
        /// The recur
        /// </summary>
        private IRecur _recur;
        /// <summary>
        /// The repeat
        /// </summary>
        private uint _repeat;
        /// <summary>
        /// The timer
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// The task wrapper
        /// </summary>
        private TaskWrapper _taskWrapper;
        /// <summary>
        /// The skip
        /// </summary>
        private bool _skip;

        /// <summary>
        /// The time zone information
        /// </summary>
        private TimeZoneInfo _timeZoneInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskRunner" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="repeat">The repeat.</param>
        /// <param name="recursion">The recursion.</param>
        /// <param name="schedule">The schedule.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="taskTime">The task time.</param>
        /// <param name="timeZoneInfo">The time zone information.</param>
        /// <param name="traceType">Type of the trace.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Repeat should be positive number for number of times to be repeated or 0 for infinite number of times
        /// or
        /// Task time should be between 00:00:00 and 23:59:59
        /// or
        /// Start date should be less than now
        /// or
        /// Schedule does not have the weekdays defined for the Weekly tasks
        /// or
        /// Schedule does not have the weekdays defined for the Weekly tasks
        /// or
        /// Schedule does not conform to the format
        /// or
        /// Schedule does not have the months defined for the Monthly tasks
        /// or
        /// Schedule does not have the weeks defined for the Monthly tasks
        /// or
        /// Schedule does not have the weekdays defined for the Monthly tasks
        /// or
        /// Schedule does not have the day defined for the Monthly tasks in a valid range
        /// or
        /// Schedule does not conform to the format
        /// or
        /// Schedule does not conform to the format
        /// or
        /// Schedule does not conform to the format
        /// </exception>
        /// <exception cref="System.ArgumentNullException">Schedule cannot be null</exception>
        public ScheduledTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, uint repeat, Recursion recursion, string schedule,
            DateTime startDate, TimeSpan taskTime, TimeZoneInfo timeZoneInfo, LogType traceType) :
            base(name, taskType, parameters, traceType)
        {

            if (repeat < 1)
                throw new ArgumentOutOfRangeException(nameof(repeat), "Repeat should be positive number for number of times to be repeated or 0 for infinite number of times");
            if (taskTime < Time.DayMinTime || taskTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(taskTime), "Task time should be between 00:00:00 and 23:59:59");
            if (schedule == null)
                throw new ArgumentNullException(nameof(schedule), "Schedule cannot be null");
            if (startDate > DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(startDate), "Start date should be less than now");
            _schedule = schedule.ToUpper();
            _recursion = recursion;
            ///If the start time is not configured set it to Sunday of this week
            _startDate = startDate == DateTime.MinValue ? DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek) : startDate;
            _repeat = repeat;
            _taskTime = taskTime;
            _timeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Local;

            switch (recursion)
            {
                case Recursion.Daily:
                    _recur = new Daily(repeat);
                    break;
                case Recursion.Weekly:
                    //WD:Tuesday, Friday, Monday
                    Weekdays weekdays = Weekdays.None;
                    if (schedule.Length < 3 || schedule.Substring(0, 3) != "WD:")
                        throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not have the weekdays defined for the Weekly tasks");

                    if (!Enum.TryParse<Weekdays>(schedule.Substring(3), true, out weekdays) || weekdays == Weekdays.None)
                        throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not have the weekdays defined for the Weekly tasks");

                    _recur = new Weekly(repeat, weekdays);
                    break;
                case Recursion.Monthly:
                    //"MN:January,March,December|WK:First,Last|WD:THursday,Friday"
                    //"MN:January,March,December|DY:1,2,3,Last"
                    Months months = Months.None;
                    weekdays = Weekdays.None;
                    Weeks week = Weeks.None;
                    uint days = 0;
                    foreach (var item in schedule.Split('|'))
                    {
                        if (item.Length < 4)
                            throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not conform to the format");
                        switch (item.Substring(0, 3))
                        {
                            case "MN:":
                                if (!Enum.TryParse<Months>(item.Substring(3), true, out months) || months == Months.None)
                                    throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not have the months defined for the Monthly tasks");
                                break;
                            case "WK:":
                                if (!Enum.TryParse<Weeks>(item.Substring(3), true, out week) || week == Weeks.None)
                                    throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not have the weeks defined for the Monthly tasks");
                                break;
                            case "WD:":
                                if (!Enum.TryParse<Weekdays>(item.Substring(3), true, out weekdays) || weekdays == Weekdays.None)
                                    throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not have the weekdays defined for the Monthly tasks");
                                break;
                            case "DY:":
                                foreach (var d in item.Substring(3).Split(','))
                                {
                                    int x = 0;
                                    uint day = 1;

                                    if (d == "LAST")
                                        days |= 0x80000000;//Last day of the month
                                    else if (d == "ALL")
                                        days |= 0x7FFFFFFF;//This is for all 1-31 days
                                    else if (!int.TryParse(d, out x) || x < 1 || x > 31)
                                        throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not have the day defined for the Monthly tasks in a valid range");
                                    else
                                        days |= day << (x - 1);
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not conform to the format");
                        }
                    }

                    if (week != Weeks.None && weekdays != Weekdays.None && months != Months.None)
                        _recur = new MonthlyByWeekdays(months, weekdays, week);
                    else if (days > 1 && months != Months.None)
                        _recur = new MonthlyByDay(months, days);
                    else
                        throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not conform to the format");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(schedule), "Schedule does not conform to the format");
            }

            _lastDateTime = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, _taskTime.Hours, _taskTime.Minutes, _taskTime.Seconds, DateTimeKind.Unspecified);

            ///If not daily task, initialize to correct start marker i.e. next schedule to align with the configuration
            ///If the task is configured for every Monday and this process starts on Thursday 
            //if (recursion != Recursion.Daily)
            //    _lastDateTime = Next(_lastDateTime);

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledTaskRunner" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="recur">The recur.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="taskTime">The task time.</param>
        /// <param name="timeZoneInfo">The time zone information.</param>
        /// <param name="traceType">Type of the trace.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Task time should be between 00:00:00 and 23:59:59</exception>
        /// <exception cref="System.ArgumentNullException">startDate should be less than now</exception>
        public ScheduledTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, IRecur recur,
            DateTime startDate, TimeSpan taskTime, TimeZoneInfo timeZoneInfo, LogType traceType) :
            base(name, taskType, parameters, traceType)
        {
            recur.NotNull(nameof(recur));
            if (taskTime < Time.DayMinTime || taskTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(taskTime), "Task time should be between 00:00:00 and 23:59:59");
            if (startDate > DateTime.Now)
                throw new ArgumentNullException("Start Date should be less than now");
            ///If the start time is not configured set it to Sunday of this week
            _startDate = startDate == DateTime.MinValue ? DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek) : startDate;
            _recur = recur;
            _taskTime = taskTime;
            _timeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Local;
            _lastDateTime = new DateTime(_startDate.Year, _startDate.Month, _startDate.Day, _taskTime.Hours, _taskTime.Minutes, _taskTime.Seconds, DateTimeKind.Unspecified);
        }


        #region TaskRunner Members        
        /// <summary>
        /// Starts this instance of task runner
        /// </summary>
        public override void Start()
        {
            lock (this)
            {
                if (_timer == null)
                {
                    _taskWrapper = new TaskWrapper(this.TaskType, new ExecutionContext(Parameters, this));

                    _timer = new Timer(new TimerCallback(RunTask), null, Timeout.Infinite, Timeout.Infinite);

                    ScheduleNextRun();

                    base.Start();
                    TraceInformation("Started", this.Name);
                }
            }
        }

        /// <summary>
        /// Stops this instance of task runner
        /// </summary>
        public override void Stop()
        {
            lock (this)
            {
                if (_timer != null)
                {
                    using (_timer) ;
                    _timer = null;
                    _taskWrapper.Release();
                    _taskWrapper = null;
                    base.Stop();
                    TraceInformation("Stopped", this.Name);
                }
            }
        }

        /// <summary>
        /// This method execute the task
        /// </summary>
        /// <param name="state"></param>
        #endregion
        private void RunTask(object state)
        {
            ///Task not started, call should never come here
            if (_timer == null)
                return;

            if (!_skip)
            {
                ExecutionState executionState = _taskWrapper.RunTask();
                TraceInformation("Executed with return value {0}", executionState);

                switch (executionState)
                {
                    case ExecutionState.Executed:
                        RaiseComplete(_taskWrapper.Context);
                        break;
                    case ExecutionState.Stop:
                        Stop();
                        return;
                    case ExecutionState.Recycle:
                        _taskWrapper.Release();
                        _taskWrapper = new TaskWrapper(this.TaskType, new ExecutionContext(Parameters, this));
                        break;
                }
            }

            ScheduleNextRun();
        }

        /// <summary>
        /// Gets the current date time for the configured time zone
        /// </summary>
        /// <value>
        /// The now.
        /// </value>
        private DateTime Now
        {
            get
            {
                DateTime now;
                if (_timeZoneInfo == null
                    || _timeZoneInfo == TimeZoneInfo.Local)
                    now = DateTime.Now;
                else
                    now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);
                TraceInformation("Now: {0}", now);
                return now;
            }
        }

        /// <summary>
        /// Schedules the next run.
        /// </summary>
        private void ScheduleNextRun()
        {
            DateTime now = Now;
            while (_lastDateTime < now)
            {
                _lastDateTime = Next(_lastDateTime);

                ///If stuck in the past date advance the date
                //_lastDateTime = dt == _lastDateTime ? _lastDateTime.AddDays(1) : dt;
            }

            TimeSpan delay = TimeZoneInfo.ConvertTimeToUtc(_lastDateTime, _timeZoneInfo) - TimeZoneInfo.ConvertTimeToUtc(now, _timeZoneInfo);

            if (delay.TotalMilliseconds > int.MaxValue)
            {
                _skip = true;
                _timer = new Timer(new TimerCallback(RunTask), null, int.MaxValue, Timeout.Infinite);
            }
            else
            {
                _skip = false;
                _timer = new Timer(new TimerCallback(RunTask), null, delay, TimeSpan.FromMilliseconds(-1));
            }
            TraceInformation("Scheduled to run next at {0}", _lastDateTime);
        }

        /// <summary>
        /// Nexts the specified from given date time
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        private DateTime Next(DateTime from)
        {
            return _recur.Next(from);
        }
    }


    /// <summary>
    /// Interface to define the recurrence for the schedule task
    /// </summary>
    public interface IRecur
    {
        /// <summary>
        /// Returns the nexts scedule from the specified date time
        /// </summary>
        /// <param name="from">From date time</param>
        /// <returns></returns>
        DateTime Next(DateTime from);
    }

    #region Daily    
    /// <summary>
    /// This is the recurrence for daily running tasks
    /// </summary>
    /// <seealso cref="XecMe.Core.Tasks.IRecur" />
    internal class Daily : IRecur
    {
        uint _repeat;

        /// <summary>
        /// Initializes a new instance of the <see cref="Daily"/> class.
        /// </summary>
        /// <param name="repeat">The repeat every other day</param>
        public Daily(uint repeat)
        {
            if (repeat < 1)
                throw new ArgumentOutOfRangeException(nameof(repeat));
            _repeat = repeat;
        }

        /// <summary>
        /// Returns the nexts scedule from the specified date time
        /// </summary>
        /// <param name="from">From date time</param>
        /// <returns></returns>
        DateTime IRecur.Next(DateTime from)
        {
            return from.AddDays(_repeat);
        }
    }
    #endregion


    #region Weekly    
    /// <summary>
    /// This is the recurrence for the weekly running date time
    /// </summary>
    /// <seealso cref="XecMe.Core.Tasks.IRecur" />
    internal class Weekly : IRecur
    {
        uint _repeat;
        Weekdays _weekdays;

        /// <summary>
        /// Initializes a new instance of the <see cref="Weekly"/> class.
        /// </summary>
        /// <param name="repeat">The repeat.</param>
        /// <param name="weekdays">The weekdays.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Should be a positive number
        /// or
        /// Should have at least a weekday defined for the task
        /// </exception>
        public Weekly(uint repeat, Weekdays weekdays)
        {
            if (repeat < 1)
                throw new ArgumentOutOfRangeException(nameof(repeat), "Should be a positive number");
            if (weekdays == Weekdays.None)
                throw new ArgumentOutOfRangeException(nameof(weekdays), "Should have at least a weekday defined for the task");
            _repeat = repeat - 1;//Multiplying with 7(week) becomes easy when we do it here
            _weekdays = weekdays;
        }

        /// <summary>
        /// Returns the nexts scedule from the specified date time
        /// </summary>
        /// <param name="from">From date time</param>
        /// <returns></returns>
        DateTime IRecur.Next(DateTime from)
        {
            DateTime original = from;
            while (true)
            {
                Weekdays thisDay = Utils.GetWeekday(from.DayOfWeek);
                ///If its all weekdays, just add the next day
                if (_weekdays == Weekdays.All)
                {
                    if (thisDay == Weekdays.Saturday)
                        from = from.AddDays((_repeat * 7) + 1);
                    else
                        from = from.AddDays(1);
                }
                else
                {
                    int d, i = (int)from.DayOfWeek;
                    ///Find number of days for the next valid weekday
                    for (d = 0; !Utils.HasWeekday(_weekdays, Utils.GetWeekday(i)); i++)
                    {
                        d++;
                    }
                    ///calculate next valid weekday to run the job
                    i = (d + (int)from.DayOfWeek) % 7;

                    ///if the next valid day is >= the from weekday simple add the day else going back to next week
                    if (i >= (int)from.DayOfWeek)
                    {
                        from = from.AddDays(d);
                    }
                    else
                    {
                        from = from.AddDays(d + (_repeat * 7));
                    }
                }
                ///If the date is same the one passed in the add a day
                if (original == from)
                {
                    from = from.AddDays(1);
                    continue;
                }
                return from;
            }
        }
    }
    #endregion

    #region Monthly by weekdays    
    /// <summary>
    /// This is the recurrence pattern for monthly run tasks
    /// </summary>
    /// <seealso cref="IRecur" />
    internal class MonthlyByWeekdays : IRecur
    {
        Months _months;
        Weekdays _weekdays;
        Weeks _weeks;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyByWeekdays"/> class.
        /// </summary>
        /// <param name="months">The months.</param>
        /// <param name="weekdays">The weekdays.</param>
        /// <param name="weeks">The weeks.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Should have at least a month defined for the task
        /// or
        /// Should have at least a weekday defined for the task
        /// or
        /// Should have at least a week defined for the task
        /// </exception>
        public MonthlyByWeekdays(Months months, Weekdays weekdays, Weeks weeks)
        {
            if (months == Months.None)
                throw new ArgumentOutOfRangeException(nameof(months), "Should have at least a month defined for the task");

            if (weekdays == Weekdays.None)
                throw new ArgumentOutOfRangeException(nameof(weekdays), "Should have at least a weekday defined for the task");

            if (weeks == Weeks.None)
                throw new ArgumentOutOfRangeException(nameof(weekdays), "Should have at least a week defined for the task");

            _months = months;
            _weekdays = weekdays;
            _weeks = weeks;
        }

        DateTime IRecur.Next(DateTime from)
        {
            DateTime original = from;
            while (true)
            {
                ///Set the valid month
                from = Utils.GetValidMonth(_months, from);

                //First week
                if ((_weeks & Weeks.First) == Weeks.First)
                {
                    while (from.Day < 8)
                    {
                        if (Utils.HasWeekday(_weekdays, Utils.GetWeekday(from.DayOfWeek))
                            && from != original)
                        {
                            return from;
                        }
                        from = from.AddDays(1);
                    }
                }
                //Second week
                if ((_weeks & Weeks.Second) == Weeks.Second)
                {
                    while (from.Day < 15 && from.Day > 7)
                    {
                        if (Utils.HasWeekday(_weekdays, Utils.GetWeekday(from.DayOfWeek))
                            && from != original)
                        {
                            return from;
                        }
                        from = from.AddDays(1);
                    }
                }
                //Third week
                if ((_weeks & Weeks.Third) == Weeks.Third)
                {
                    while (from.Day < 22 && from.Day > 14)
                    {
                        if (Utils.HasWeekday(_weekdays, Utils.GetWeekday(from.DayOfWeek))
                            && from != original)
                        {
                            return from;
                        }
                        from = from.AddDays(1);
                    }
                }
                //Both Fourth & Last week
                if ((_weeks & (Weeks.Fourth | Weeks.Last)) == (Weeks.Fourth | Weeks.Last))
                {
                    while (from.Day > 21)
                    {
                        if (Utils.HasWeekday(_weekdays, Utils.GetWeekday(from.DayOfWeek))
                            && from != original)
                        {
                            return from;
                        }
                        from = from.AddDays(1);
                    }
                }
                else
                {
                    //Fourth week
                    if ((_weeks & Weeks.Fourth) == Weeks.Fourth)
                    {
                        while (from.Day < 29 && from.Day > 21)
                        {
                            if (Utils.HasWeekday(_weekdays, Utils.GetWeekday(from.DayOfWeek))
                                && from != original)
                            {
                                return from;
                            }
                            from = from.AddDays(1);
                        }
                    }
                    //Last
                    if ((_weeks & Weeks.Last) == Weeks.Last)
                    {
                        int lastWeekDay = GetLastWeekStartDay(from);
                        while (from.Day >= lastWeekDay)
                        {
                            if (Utils.HasWeekday(_weekdays, Utils.GetWeekday(from.DayOfWeek))
                                && from != original)
                            {
                                return from;
                            }
                            from = from.AddDays(1);
                        }
                    }
                }
                from = from.AddDays(1);
            }
        }

        /// <summary>
        /// Gets the last week start day.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private int GetLastWeekStartDay(DateTime date)
        {
            switch (date.Month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return 25;
                case 4:
                case 6:
                case 9:
                case 11:
                    return 24;
                case 2:
                    return DateTime.IsLeapYear(date.Year) ? 23 : 22;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    #endregion


    #region Monthly by day    
    /// <summary>
    /// This is the recurrence pattern that schedules the task monthly by day
    /// </summary>
    /// <seealso cref="IRecur" />
    internal class MonthlyByDay : IRecur
    {
        const uint LAST = 0x80000000;
        Months _months;
        uint _days;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonthlyByDay"/> class.
        /// </summary>
        /// <param name="months">The months.</param>
        /// <param name="days">The days.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Should have at least a month defined for the task
        /// </exception>
        public MonthlyByDay(Months months, uint days)
        {
            if (days < 1 || days > 0xFFFFFFFF)
                throw new ArgumentOutOfRangeException(nameof(days));

            if (months == Months.None)
                throw new ArgumentOutOfRangeException(nameof(months), "Should have at least a month defined for the task");

            _days = days;
            _months = months;
        }

        /// <summary>
        /// Returns the nexts scedule from the specified date time
        /// </summary>
        /// <param name="from">From date time</param>
        /// <returns></returns>
        DateTime IRecur.Next(DateTime from)
        {
            DateTime original = from;
            while (true)
            {
                from = Utils.GetValidMonth(_months, from);

                ///Save the month
                int month = from.Month;
                ///Iterate while the month is same
                while (month == from.Month)
                {
                    ///if its a valid day return and not same date
                    if (HasDay(from.Day) && from != original)
                        return from;

                    ///Check the next day
                    from = from.AddDays(1);
                }
                //Month changed, so check the last day now, else go for the next valid month
                if ((_days & LAST) > 0)
                {
                    return from.AddDays(-1);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified day has day.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns>
        ///   <c>true</c> if the specified day has day; otherwise, <c>false</c>.
        /// </returns>
        private bool HasDay(int day)
        {
            uint x = 1;
            x <<= (day - 1);
            return (x & _days) > 0;
        }
    }
    #endregion
    /// <summary>
    /// Helper functions for the date time calculations
    /// </summary>
    static class Utils
    {
        /// <summary>
        /// Returns the input date time if current month is valid month else set to the 1st of the next valid month
        /// </summary>
        /// <param name="months">Valid months</param>
        /// <param name="from">DateTime to be checked for valid months</param>
        /// <returns></returns>
        public static DateTime GetValidMonth(Months months, DateTime from)
        {
            ///Check if this is a valid month else loop until you get to the valid month
            while (!Utils.HasMonth(months, Utils.GetMonth(from.Month)))
            {
                ///If its not the first day of the month, the set the day to the first day of the month
                if (from.Day > 1)
                    from = from.AddDays(-from.Day + 1);//Get to the first of this month

                ///add a month until you get to the valid month
                from = from.AddMonths(1);
            }
            return from;
        }
        /// <summary>
        /// Maps the month number to the month enum value of XecMe framework
        /// </summary>
        /// <param name="month">Enum value of the month</param>
        /// <returns></returns>
        public static Months GetMonth(int month)
        {
            if (month < 1 || month > 12)
                throw new ArgumentOutOfRangeException("month");
            month--;
            
            return (Months)(1 << month);
        }

        /// <summary>
        /// Maps the numeric weekday value to the XecMe framework weekday
        /// </summary>
        /// <param name="dayOfWeek">numeric value of the weekday</param>
        /// <returns>Weekday enum value</returns>
        public static Weekdays GetWeekday(int dayOfWeek)
        {
            return (Weekdays)(1 << (dayOfWeek % 7));
        }

        /// <summary>
        /// Maps the .NET weekday to the XecMe framework weekday
        /// </summary>
        /// <param name="dayOfWeek">.NET enum weekday value</param>
        /// <returns>Weekday enum value</returns>
        public static Weekdays GetWeekday(DayOfWeek dayOfWeek)
        {
            return (Weekdays)Enum.Parse(typeof(Weekdays), dayOfWeek.ToString());
        }

        /// <summary>
        /// Returns true if the given month is in the months
        /// </summary>
        /// <param name="months">Configured months</param>
        /// <param name="month">Specific month</param>
        /// <returns>True, if the specific month is in the months</returns>
        public static bool HasMonth(Months months, Months month)
        {
            return month == (months & month);
        }

        /// <summary>
        /// Returns true if the given weekday is in the weekdays
        /// </summary>
        /// <param name="months">Configured weekdays</param>
        /// <param name="month">Specific weekday</param>
        /// <returns>True, if the specific weekday is in the weekdays</returns>
        public static bool HasWeekday(Weekdays weekdays, Weekdays weekday)
        {
            return weekday == (weekdays & weekday);
        }
    }

}
