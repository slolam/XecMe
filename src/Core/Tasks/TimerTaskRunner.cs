#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TimerTaskRunner.cs is part of XecMe.
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
/// Added           01-2013             Shailesh Lolam      Added time zone to the timer for Azure

#endregion
using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Core.Tasks;
using System.Threading;
using XecMe.Core.Utils;
using System.Collections.Specialized;
using System.Diagnostics;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Timer task runner
    /// </summary>
    /// <seealso cref="XecMe.Core.Tasks.TaskRunner" />
    public class TimerTaskRunner : TaskRunner
    {
        /// <summary>
        /// The interval
        /// </summary>
        private long _interval;
        /// <summary>
        /// The recurrence/
        /// </summary>
        private long _recurrence;
        /// <summary>
        /// The start date time
        /// </summary>
        private DateTime _startDateTime = DateTime.MinValue;
        /// <summary>
        /// The end date time
        /// </summary>
        private DateTime _endDateTime = DateTime.MaxValue;
        /// <summary>
        /// The day start time
        /// </summary>
        private TimeSpan _dayStartTime = TimeSpan.FromSeconds(0);
        /// <summary>
        /// The day end time
        /// </summary>
        private TimeSpan _dayEndTime = TimeSpan.FromSeconds(86399);
        /// <summary>
        /// The weekdays
        /// </summary>
        private Weekdays _weekdays = Weekdays.All;
        /// <summary>
        /// The timer
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// The task
        /// </summary>
        private TaskWrapper _task;
        /// <summary>
        /// The time zone information
        /// </summary>
        private TimeZoneInfo _timeZoneInfo;


        /// <summary>
        /// Initializes a new instance of the <see cref="TimerTaskRunner"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="recurrence">The recurrence.</param>
        /// <param name="weekdays">The weekdays.</param>
        /// <param name="startDateTime">The start date time.</param>
        /// <param name="endDateTime">The end date time.</param>
        /// <param name="dayStartTime">The day start time.</param>
        /// <param name="dayEndTime">The day end time.</param>
        /// <param name="timeZoneInfo">The time zone information.</param>
        /// <param name="traceType">Type of the trace.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Interval cannot be less than 1
        /// or
        /// Recurrence has to be greater than 0 or euqal to -1
        /// or
        /// Start date time should be less than End date time
        /// or
        /// Start time cannot be negative
        /// or
        /// End time cannot be negative
        /// or
        /// Start time should be less than 23:59:59
        /// or
        /// End time should be less than 23:59:59
        /// or
        /// No weekday is selected to run the task
        /// </exception>
        public TimerTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, long interval, long recurrence,
            Weekdays weekdays, DateTime startDateTime, DateTime endDateTime, TimeSpan dayStartTime, TimeSpan dayEndTime, 
            TimeZoneInfo timeZoneInfo, LogType traceType) :
            base(name, taskType, parameters, traceType)
        {
            /*}
            public TimerTaskRunner(string name, Type taskType, StringDictionary parameters, long interval, long recurrence,
                DateTime startDateTime, DateTime endDateTime, TimeSpan dayStartTime, TimeSpan dayEndTime, TimeZoneInfo timeZoneInfo) :
                base(name, taskType, parameters)
            {*/
            if (interval < 1)
                throw new ArgumentOutOfRangeException(nameof(interval), "Interval cannot be less than 1");

            if (recurrence != -1 && recurrence < 1)
                throw new ArgumentOutOfRangeException(nameof(recurrence), "Recurrence has to be greater than 0 or euqal to -1");

            if (endDateTime < startDateTime)
                throw new ArgumentOutOfRangeException(nameof(startDateTime), "Start date time should be less than End date time");

            if (dayStartTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException(nameof(dayStartTime), "Start time cannot be negative");

            if (dayEndTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException(nameof(dayEndTime), "End time cannot be negative");

            if (dayStartTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(dayStartTime), "Start time should be less than 23:59:59");

            if (dayEndTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(dayEndTime), "End time should be less than 23:59:59");

            if (weekdays == Weekdays.None)
                throw new ArgumentOutOfRangeException(nameof(weekdays), "No weekday is selected to run the task");

            _weekdays = weekdays;
            _timeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Local;
            _interval = interval;
            _recurrence = recurrence;
            _startDateTime = startDateTime;
            _endDateTime = endDateTime;
            _dayStartTime = dayStartTime;
            _dayEndTime = dayEndTime;
        }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public long Interval
        {
            get
            {
                return _interval;
            }
        }

        /// <summary>
        /// Gets the recurrence.
        /// </summary>
        /// <value>
        /// The recurrence.
        /// </value>
        public long Recurrence
        {
            get
            {
                return _recurrence;
            }
        }

        /// <summary>
        /// Gets the start date time.
        /// </summary>
        /// <value>
        /// The start date time.
        /// </value>
        public DateTime StartDateTime
        {
            get
            {
                return _startDateTime;
            }
        }

        /// <summary>
        /// Gets the end date time.
        /// </summary>
        /// <value>
        /// The end date time.
        /// </value>
        public DateTime EndDateTime
        {
            get
            {
                return _endDateTime;
            }
        }

        /// <summary>
        /// Gets the day start time.
        /// </summary>
        /// <value>
        /// The day start time.
        /// </value>
        public TimeSpan DayStartTime
        {
            get
            {
                return _dayStartTime;
            }
        }

        /// <summary>
        /// Gets the day end time.
        /// </summary>
        /// <value>
        /// The day end time.
        /// </value>
        public TimeSpan DayEndTime
        {
            get
            {
                return _dayEndTime;
            }
        }

        /// <summary>
        /// Gets the time zone.
        /// </summary>
        /// <value>
        /// The time zone.
        /// </value>
        public TimeZoneInfo TimeZone
        {
            get
            {
                return _timeZoneInfo;
            }
        }


        #region TaskRunner Members        
        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            lock (this)
            {
                if (_timer == null)
                {
                    _task = new TaskWrapper(this.TaskType, new ExecutionContext(Parameters, this));
                    _timer = new Timer(new TimerCallback(RunTask), null, Interval, Interval);
                    base.Start();
                    TraceInformation("Started");
                }
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public override void Stop()
        {
            lock (this)
            {
                if (_timer != null)
                {
                    using (_timer) ;
                    _timer = null;
                    _task.Release();
                    _task = null;
                    base.Stop();
                    TraceInformation("Stopped");
                }
            }
        }

        #endregion        
        /// <summary>
        /// Gets the now.
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
        /// Runs the task.
        /// </summary>
        /// <param name="state">The state.</param>
        private void RunTask(object state)
        {
            ///Task not started, call should never come here
            if (_timer == null)
                return;

            DateTime today = Now;
            ///Not in the limit of the date time range
            if (today > _endDateTime)
            {
                Stop();
                return;
            }

            ///Sleep until the interval duration before start time
            if (today < _startDateTime)
            {
                TimeSpan wait = TimeZoneInfo.ConvertTimeToUtc(_startDateTime, _timeZoneInfo).Subtract(TimeZoneInfo.ConvertTimeToUtc(today, _timeZoneInfo));
                TraceInformation("will wait for {0} before it starts", wait);
                _timer.Change((long)wait.TotalMilliseconds, _interval);
                return;
            }

            ///Not in the range of time of the day or avalid weekday
            if (Time.Disallow(today, _dayStartTime, _dayEndTime, _weekdays))
            {
                DateTime st = new DateTime(today.Year, today.Month, today.Day, _dayStartTime.Hours, _dayStartTime.Minutes, _dayStartTime.Seconds, DateTimeKind.Unspecified);

                ///If its not an allowed weekday, wait until next start of day
                if (!Utils.HasWeekday(_weekdays, Utils.GetWeekday(today.DayOfWeek)) || today > st)
                    st = st.AddDays(1).Date;

                ///Find the differene by converting to UTC time to make sure daylight cutover are accounted
                TimeSpan wait = TimeZoneInfo.ConvertTimeToUtc(st, _timeZoneInfo) - TimeZoneInfo.ConvertTimeToUtc(today, _timeZoneInfo);

                //TimeSpan wait = st.Subtract(now);
                TraceInformation("will wait for {0} before it starts", wait);
                _timer.Change((long)wait.TotalMilliseconds, _interval);
                return;
            }

            if (_recurrence > 0 || _recurrence == -1)
            {
                ///Stop the timer
                _timer.Change(Timeout.Infinite, _interval);
                ExecutionState executionState = _task.RunTask();

                switch (executionState)
                {
                    case ExecutionState.Executed:
                        RaiseComplete(_task.Context);
                        break;
                    case ExecutionState.Stop:
                        _task.Release();
                        Stop();
                        return;
                    case ExecutionState.Recycle:
                        _task.Release();
                        _task = new TaskWrapper(this.TaskType, new ExecutionContext(Parameters, this));
                        break;
                }

                if (_recurrence != -1)
                    _recurrence--;

                ///Resume the timer
                _timer.Change(Interval, _interval);
            }
            else
            {
                Stop();
                TraceInformation("stopped because the reccurence is 0 ");
            }
        }
    }
}

