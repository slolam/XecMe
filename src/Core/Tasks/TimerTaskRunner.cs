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
    public class TimerTaskRunner : TaskRunner
    {
        private long _interval;
        private long _recurrence;
        private DateTime _startDateTime = DateTime.MinValue;
        private DateTime _endDateTime = DateTime.MaxValue;
        private TimeSpan _dayStartTime = TimeSpan.FromSeconds(0);
        private TimeSpan _dayEndTime = TimeSpan.FromSeconds(86399);
        private Weekdays _weekdays = Weekdays.All;
        private Timer _timer;
        private TaskWrapper _task;
        private TimeZoneInfo _timeZoneInfo;

        
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
                throw new ArgumentOutOfRangeException(nameof(interval), $"{nameof(interval)} cannot be less than 1");

            if (recurrence != -1 && recurrence < 1)
                throw new ArgumentOutOfRangeException(nameof(recurrence), $"{nameof(recurrence)} has to be greater than 0 or euqal to -1");

            if (endDateTime < startDateTime)
                throw new ArgumentOutOfRangeException(nameof(startDateTime), $"{nameof(startDateTime)} should be less than {nameof(endDateTime)}");

            if (dayStartTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException(nameof(dayStartTime), $"{nameof(dayStartTime)} cannot be negative");

            if (dayEndTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException(nameof(dayEndTime), $"{nameof(dayEndTime)} cannot be negative");

            if (dayStartTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(dayStartTime), $"{nameof(dayStartTime)} should be less than 23:59:59");

            if (dayEndTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException(nameof(dayEndTime), $"{nameof(dayEndTime)} should be less than 23:59:59");

            if (weekdays == Weekdays.None)
                throw new ArgumentOutOfRangeException(nameof(weekdays), "No weekday is selected to run the task");

            //if (dayEndTime < dayStartTime)
            //    throw new ArgumentOutOfRangeException("dayStartTime", "dayStartTime should be less than dayEndTime");

            _weekdays = weekdays;
            _timeZoneInfo = timeZoneInfo ?? TimeZoneInfo.Local;
            _interval = interval;
            _recurrence = recurrence;
            _startDateTime = startDateTime;
            _endDateTime = endDateTime;
            _dayStartTime = dayStartTime;
            _dayEndTime = dayEndTime;
        }

        public long Interval
        {
            get
            {
                return _interval;
            }
        }
        public long Recurrence
        {
            get
            {
                return _recurrence;
            }
        }
        public DateTime StartDateTime
        {
            get
            {
                return _startDateTime;
            }
        }

        public DateTime EndDateTime
        {
            get
            {
                return _endDateTime;
            }
        }

        public TimeSpan DayStartTime
        {
            get
            {
                return _dayStartTime;
            }
        }
        public TimeSpan DayEndTime
        {
            get
            {
                return _dayEndTime;
            }
        }
        public TimeZoneInfo TimeZone
        {
            get
            {
                return _timeZoneInfo;
            }
        }


        #region TaskRunner Members

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

