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

namespace XecMe.Core.Tasks
{
    public class TimerTaskRunner : TaskRunner
    {
        public long Interval { get; set; }
        public long Recurrence { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan DayStartTime { get; set; }
        public TimeSpan DayEndTime { get; set; }
        private Timer _timer;
        private TaskWrapper _task;

        public TimerTaskRunner(string name, Type taskType, StringDictionary parameters, long interval, long recurrence) :
            base(name, taskType, parameters)
        {
            Interval = interval;
            Recurrence = recurrence;
        }

        #region TaskRunner Members

        public override void Start()
        {
            _task = new TaskWrapper(this.GetTaskInstance(), new ExecutionContext(Parameters));
            _timer = new Timer(new TimerCallback(RunTask), null, Interval, Interval);
            base.Start();
        }

        public override void Stop()
        {
            using (_timer) ;
            _task.Release();
            _task = null;
            base.Stop();
        }

        #endregion

        private void RunTask(object state)
        {
            DateTime today = DateTime.Now;
            ///Not in the limit of the date time range
            if (today < StartDateTime || today > EndDateTime)
                return;
            

            TimeSpan now = DateTime.Now.TimeOfDay;
            ///Not in the range of time of the day
            if (now < DayStartTime || now > DayEndTime)
                return;

            if (Recurrence > 0 || Recurrence == -1)
            {
                ///Stop the timer
                _timer.Change(Timeout.Infinite, Interval);
                ExecutionState executionState = _task.RunTask();
                switch (executionState)
                {
                    case ExecutionState.Stop:
                        Stop();
                        return;
                    case ExecutionState.Recycle:
                        _task = new TaskWrapper(this.GetTaskInstance(), new ExecutionContext(Parameters, this));
                        break;
                }

                if (Recurrence != -1)
                    Recurrence--;

                ///Resume the timer
                _timer.Change(Interval, Interval);

            }
        }
    }
}
