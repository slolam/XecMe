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

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly TimeSpan TS_MIN;
        private static readonly TimeSpan TS_MAX;
        private long _interval;
        private long _recurrence;
        private DateTime _startDateTime = DateTime.MinValue;
        private DateTime _endDateTime = DateTime.MaxValue;
        private TimeSpan _dayStartTime = TimeSpan.FromSeconds(0);
        private TimeSpan _dayEndTime = TimeSpan.FromSeconds(86399);
        private Timer _timer;
        private TaskWrapper _task;

        static TimerTaskRunner()
        {
            TS_MIN = TimeSpan.FromSeconds(0.0);
            TS_MAX = TimeSpan.FromSeconds(86399.0);
        }

        public TimerTaskRunner(string name, Type taskType, StringDictionary parameters, long interval, long recurrence,
            DateTime startDateTime, DateTime endDateTime, TimeSpan dayStartTime, TimeSpan dayEndTime) :
            base(name, taskType, parameters)
        {
            if (interval < 1)
                throw new ArgumentOutOfRangeException("interval", "interval cannot be less than 1");

            if (recurrence != -1 && recurrence < 1)
                throw new ArgumentOutOfRangeException("recurrence", "recurrence has to be greater than 0 or euqal to -1");

            if (endDateTime < startDateTime)
                throw new ArgumentOutOfRangeException("startDateTime", "startDateTime should be less than endDateTime");

            if (dayStartTime < TS_MIN)
                throw new ArgumentOutOfRangeException("dayStartTime", "dayStartTime cannot be negative");

            if (dayEndTime > TS_MAX)
                throw new ArgumentOutOfRangeException("dayEndTime", "dayEndTime should be less than 23:59:59");

            if (dayEndTime < dayStartTime)
                throw new ArgumentOutOfRangeException("dayStartTime", "dayStartTime should be less than dayEndTime");

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

        
        #region TaskRunner Members

        public override void Start()
        {
            lock (this)
            {
                if (_timer == null)
                {
                    _task = new TaskWrapper(this.GetTaskInstance(), new ExecutionContext(Parameters));
                    _timer = new Timer(new TimerCallback(RunTask), null, Interval, Interval);
                    base.Start();
                    Trace.TraceInformation("Timer Task \"{0}\" started", this.Name);
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
                    Trace.TraceInformation("Timer Task \"{0}\" stopped", this.Name);
                }
            }
        }

        #endregion

        private void RunTask(object state)
        {
            ///Task not started, call should never come here
            if (_timer == null)
                return;

            DateTime today = DateTime.Now;
            ///Not in the limit of the date time range
            if (today > _endDateTime)
            {
                Stop();
                return;
            }
            ///Sleep until the 5 secs before start time
            TimeSpan wait = _startDateTime.Subtract(today).Add(TimeSpan.FromMilliseconds(-_interval - 5000));
            //if (today.AddMilliseconds(-(Interval + 5000)) < _startDateTime)
            if (wait.TotalMilliseconds > 0)
            {
                Trace.TraceInformation("Timer task \"{0}\" will wait for {1} before it starts", this.Name, wait);
                _timer.Change((long)wait.TotalMilliseconds, _interval);
                return;
            }
            if (today < _startDateTime || today > _endDateTime)
                return;


            TimeSpan now = DateTime.Now.TimeOfDay;
            ///Not in the range of time of the day
            wait = _dayStartTime.Subtract(now.Add(TimeSpan.FromSeconds(5)));
            //if (now.Add(TimeSpan.FromSeconds(-5)) < _dayStartTime)
            if (wait.TotalMilliseconds > 0)
            {
                Trace.TraceInformation("Timer task \"{0}\" will wait for {1} before it starts", this.Name, wait);
                _timer.Change((long)wait.TotalMilliseconds, _interval);
                return;
            }
            if (now > _dayEndTime)
            {
                wait = _dayStartTime.Subtract(now.Subtract(TimeSpan.FromSeconds(86405)));
                Trace.TraceInformation("Timer task \"{0}\" will wait for {1} before it starts", this.Name, wait);
                _timer.Change((long)wait.TotalMilliseconds, _interval);
                return;
            }

            if (now < _dayStartTime || now > _dayEndTime)
                return;

            if (_recurrence > 0 || _recurrence == -1)
            {
                ///Stop the timer
                _timer.Change(Timeout.Infinite, _interval);
                ExecutionState executionState = _task.RunTask();

                Trace.TraceInformation("Timer Task \"{0}\" executed with return value {1}", this.Name, executionState);

                switch (executionState)
                {
                    case ExecutionState.Executed:
                        RaiseComplete(_task.Context);
                        break;
                    case ExecutionState.Stop:
                        Stop();
                        return;
                    case ExecutionState.Recycle:
                        _task = new TaskWrapper(this.GetTaskInstance(), new ExecutionContext(Parameters, this));
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
                Trace.TraceInformation("Timer Task \"{0}\" is stopped because the reccurence is 0 ", this.Name);
            }
        }
    }
}

