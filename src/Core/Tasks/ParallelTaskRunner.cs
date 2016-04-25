#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ParallelTaskRunner.cs is part of XecMe.
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
using System.Text;
using XecMe.Core.Tasks;
using XecMe.Core.Utils;
using System.Threading;
using System.Collections.Specialized;
using System.Diagnostics;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// This class runs the parallel instances of task based on the peocessing power of the process
    /// </summary>
    public class ParallelTaskRunner : TaskRunner
    {
        /// <summary>
        /// This hold the list of the free TaskWrapper instances created.
        /// </summary>
        private List<TaskWrapper> _freeTaskPool = new List<TaskWrapper>();
        /// <summary>
        /// This holds the list of the total TaskWrapper instances created.
        /// </summary>
        private List<TaskWrapper> _allTasks = new List<TaskWrapper>();
        /// <summary>
        /// Total no. of jobs in the queue waiting to be executed.
        /// </summary>
        private uint _jobsInQueue;
        /// <summary>
        /// Minimum no. of tasks that should run in parallel
        /// </summary>
        /// <remarks>Although this is the configuration, the actual no. of tasks running 
        /// depends on lot of other factors</remarks>
        private uint _minInstances;
        /// <summary>
        /// Maximum no. of tasks that should run in parallel
        /// </summary>
        /// <remarks>Although this is the configuration, the actual no. of tasks running 
        /// depends on lot of other factors</remarks>
        private uint _maxInstances;
        /// <summary>
        /// This is the time in milliseconds the thread will sleep when there is no work
        /// </summary>
        private ulong _idlePollingPeriod;
        /// <summary>
        /// This indicate the total no. of task actually running in parallel at a given instance.
        /// </summary>
        private uint _parallelInstances;
        /// <summary>
        /// Indicator to indicate whether the ParallelTaskRunner is running or not.
        /// </summary>
        private bool _isStarted;
        /// <summary>
        /// Synchronization object for internal use.
        /// </summary>
        private object _sync = new object();
        /// <summary>
        /// This is the timer used for delaying in case of idling.
        /// </summary>
        private Timer _timer;
        /// <summary>
        /// This is the shared execution context across all the instances
        /// </summary>
        private ExecutionContext _sharedExecutionContext;
        /// <summary>
        /// Indicates whether the attached task is singleton.
        /// </summary>
        //private TaskWrapper _singletonInstance;
        /// <summary>
        /// Day of the week when this task is allowed to run, it defaults to Weekdays.All i.e. all days of the week
        /// </summary>
        private Weekdays _weekdays = Weekdays.All;
        /// <summary>
        /// Start time of the task in a given valid day before which the task cannot run
        /// </summary>
        private TimeSpan _dayStartTime = TimeSpan.FromSeconds(0);
        /// <summary>
        /// End time on a given valid day after which the task cannot run
        /// </summary>
        private TimeSpan _dayEndTime = TimeSpan.FromSeconds(86400);
        /// <summary>
        /// Time zone reference to be used for all the time calculations. Very useful for all cloud based use.
        /// </summary>
        private TimeZoneInfo _timeZoneInfo;

        public ParallelTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, uint min, uint max, ulong idlePollingPeriod,
            TimeSpan startTime, TimeSpan endTime, Weekdays weekdays, TimeZoneInfo timeZoneInfo, TraceType traceType) :
            base(name, taskType, parameters, traceType)
        {
            if (min < 1 || max < min)
                throw new ArgumentOutOfRangeException("min", "min and max should be greater than 1 and min should be less than or equal to max");

            if (idlePollingPeriod < 100)
                throw new ArgumentOutOfRangeException("max", "idlePollingPeriod should be at least 100ms.");

            //if (endTime < startTime)
            //    throw new ArgumentOutOfRangeException("startTime", "startDateTime should be less than endDateTime");

            if (startTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException("dayStartTime", "dayStartTime cannot be negative");

            if (endTime < Time.DayMinTime)
                throw new ArgumentOutOfRangeException("dayEndTime", "dayEndTime cannot be negative");
            
            if (startTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException("dayStartTime", "dayStartTime should be less than 23:59:59");

            if (endTime > Time.DayMaxTime)
                throw new ArgumentOutOfRangeException("dayEndTime", "dayEndTime should be less than 23:59:59");

            _dayStartTime = startTime;
            _dayEndTime = endTime;
            _weekdays = weekdays;
            _timeZoneInfo = timeZoneInfo??TimeZoneInfo.Local;
            _minInstances = min;
            _maxInstances = max;
            _idlePollingPeriod = idlePollingPeriod;
            _jobsInQueue = 0;
        }

        /// <summary>
        /// Idle polling period in millisecond, this is the time the thread will sleep before attempting next time.
        /// </summary>
        /// <remarks>This is to reduce the load on the process when there is no work</remarks>
        public ulong IdlePollingPeriod
        {
            get { return _idlePollingPeriod; }
        }

        /// <summary>
        /// This is the minimum number of instances that will work in parallel
        /// </summary>
        public uint MinInstances
        {
            get { return _minInstances; }
        }

        /// <summary>
        /// This is the maximun number of instance that will work in parallel in case of full load.
        /// </summary>
        /// <remarks>Although this defines the maximum parallel instance, but it depends on other factors
        /// like load, cpu utilization, cpu allocation to the process etc.</remarks>
        public uint MaxInstances
        {
            get { return _maxInstances; }
        }

        /// <summary>
        /// Gets the number of instances that are running in parallel.
        /// </summary>
        public uint ParallelInstances
        {
            get { return _parallelInstances; }
        }

        #region TaskRunner Members
        public override void Start()
        {
            lock (_sync)
            {
                if (!_isStarted)
                {
                    _sharedExecutionContext = new ExecutionContext(Parameters, this);
                    AddTask(MinInstances);
                    _isStarted = true;
                    _jobsInQueue = 0;
                    _parallelInstances = 0;
                    QueueTask();
                    base.Start();
                    TraceInformation("Started");
                }
            }
        }

        public override void Stop()
        {
            lock (_sync)
            {
                if (_isStarted)
                {
                    while (_freeTaskPool.Count > 0)
                    {
                        TaskWrapper taskWrapper = _freeTaskPool[0];
                        _freeTaskPool.RemoveAt(0);
                        _allTasks.Remove(taskWrapper);
                        taskWrapper.Release();
                    }
                    while (_allTasks.Count > 0)
                    {
                        TaskWrapper taskWrapper = _allTasks[0];
                        _allTasks.RemoveAt(0);
                        taskWrapper.Release();
                    }
                    using (_timer) ;
                    _timer = null;
                    _jobsInQueue = 0;
                    _parallelInstances = 0;
                    _isStarted = false;
                    base.Stop();
                    TraceInformation("Stopped", this.Name);
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
            if (!_isStarted)
                return;

            TaskWrapper taskWrapper = null;
            lock (_sync)
            {
                // if this is coming as part of the timer trigger close down the timer
                if (_timer != null)
                {
                    TraceInformation("Disabling the timer");
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                }

                _jobsInQueue--;

                DateTime today = Now;
                TimeSpan now = today.TimeOfDay;

                ///Check whether it is within the allowed time
                if (Time.Disallow(today, _dayStartTime, _dayEndTime, _weekdays))
                {
                    TraceInformation("Not allowed to run at this time");

                    ///If there are more queued items, release this thread task
                    if (_jobsInQueue > 0)
                    {
                        TraceInformation("Have more items in queue");
                        return;
                    }

                    ///If the number of thread running is more that minimum thread, remove 1 and return;
                    if (_freeTaskPool.Count > 0 && _allTasks.Count > MinInstances)
                    {
                        TraceInformation("Releasing a task");
                        taskWrapper = _freeTaskPool[0];
                        taskWrapper.Release();
                        _freeTaskPool.Remove(taskWrapper);
                        _allTasks.Remove(taskWrapper);
                    }


                    if (_timer == null)
                    {
                        TraceInformation("Timer created");
                        _timer = new Timer(new TimerCallback(RunTask), null, Timeout.Infinite, Timeout.Infinite);
                    }
                    
                    ///Do it only for the last thread.
                    if (_parallelInstances == 0 && _allTasks.Count == MinInstances)
                    {
                        DateTime st = new DateTime(today.Year, today.Month, today.Day, _dayStartTime.Hours, _dayStartTime.Minutes, _dayStartTime.Seconds, DateTimeKind.Unspecified);

                        ///if today is not allowed weekday, move it to next start of the day
                        if (!Utils.HasWeekday(_weekdays, Utils.GetWeekday(today.DayOfWeek)) || today > st)
                            st = st.AddDays(1).Date; //Setting it to next day 12:00 AM
                        
                        ///Find the differene by converting to UTC time to make sure daylight cutover are accounted
                        TimeSpan wait = TimeZoneInfo.ConvertTimeToUtc(st, _timeZoneInfo) - TimeZoneInfo.ConvertTimeToUtc(today, _timeZoneInfo);
                        
                        ///This is for the timer job
                        _jobsInQueue++;
                        _timer.Change((long)wait.TotalMilliseconds, Timeout.Infinite);
                        //_timer.Change(_idlePollingPeriod, Timeout.Infinite);
                        TraceInformation("wait for {0} before it starts", wait);
                    }
                    return;
                }

                if (_freeTaskPool.Count == 0)
                    AddTask(1);

                if (_freeTaskPool.Count > 0)
                {
                    taskWrapper = _freeTaskPool[0];
                    _freeTaskPool.RemoveAt(0);
                }
                else
                {
                    ///There are no parallel capacity so exit
                    TraceInformation("There is no more thread capacity for this task runner");
                    return;
                }

                _parallelInstances++;
            }
            TraceInformation("Free Task {0}, All Task {1}, Parallel Instanse {2}, Queue {3}", _freeTaskPool.Count, _allTasks.Count, _parallelInstances, _jobsInQueue);

            ExecutionState executionState = taskWrapper.RunTask();

            switch (executionState)
            {
                case ExecutionState.Executed:
                    //Since there is work it should also create one more instance for parallel tasking
                    lock (_sync)
                    {
                        ///Reduce the number of instances
                        _parallelInstances--;
                        ///Put back the task into the free pool
                        _freeTaskPool.Add(taskWrapper);
                        //Since executed successfully it should go back for execution
                        QueueTask();
                        //there is more capacity possible, put one more to work
                        if (_jobsInQueue + _parallelInstances < this.MaxInstances)
                        {
                            QueueTask();
                        }
                    }
                    RaiseComplete(taskWrapper.Context);
                    break;
                case ExecutionState.Stop:
                    //Since the task has indicated to stop, release it
                    lock (_sync)
                    {
                        ///Reduce the number of instances
                        _parallelInstances--;
                        _allTasks.Remove(taskWrapper);
                        _freeTaskPool.Remove(taskWrapper);
                        ///If that the last instance then stop the task
                        if (_jobsInQueue == 0 && _parallelInstances == 1)
                            Stop();
                    }
                    break;
                case ExecutionState.Recycle:
                    //Since the task has indicated to recycle it, release it and add new.
                    lock (_sync)
                    {
                        ///Reduce the number of instances
                        _parallelInstances--;

                        _allTasks.Remove(taskWrapper);
                        _freeTaskPool.Remove(taskWrapper);
                        AddTask(1);
                        QueueTask();
                    }
                    break;
                case ExecutionState.Idle:
                case ExecutionState.Exception://If there is an unhandled exception we should treat it as idle else it will lead to CPU racing
                    //This is tough task :-), not enough work to do, start firing ;-)
                    lock (_sync)
                    {
                        ///Reduce the number of instances
                        _parallelInstances--;

                        ///Put back the task into the free pool
                        _freeTaskPool.Add(taskWrapper);
                        //There are other items in the queue do nothing come out.
                        if (_jobsInQueue > 0)
                            break;

                        ///Fire 1 task, save resources ONLY if task return Idle
                        if (_allTasks.Count > MinInstances)
                        {
                            taskWrapper = _freeTaskPool[0];
                            taskWrapper.Release();
                            _freeTaskPool.Remove(taskWrapper);
                            _allTasks.Remove(taskWrapper);
                        }
                        ///This is the last thread, put a timer since there is no work, lets wait for 5 seconds.
                        ///This can be further imporved by having increasing delay i.e. wait for 5, 10, 15 when we 
                        ///get work reset it to 5 secs
                        if (_timer == null)
                            _timer = new Timer(new TimerCallback(RunTask), null, Timeout.Infinite, Timeout.Infinite);
                        ///Wait for idle period to see whether there is something to do
                        ///Do it only for the last thread and there is no job in the queue
                        if (_parallelInstances == 0 && _jobsInQueue == 0)
                        {
                            ///This is for the timer job
                            _jobsInQueue++;
                            _timer.Change((long)_idlePollingPeriod, Timeout.Infinite);
                            TraceInformation("Is in timer mode");
                        }
                    }
                    break;
                default:
                    ///This could be for any new state
                    lock (_sync)
                    {
                        ///Reduce the number of instances
                        _parallelInstances--;
                        ///Put back the task into the free pool
                        _freeTaskPool.Add(taskWrapper);
                        QueueTask();
                    }
                    break;
            }
        }

        /// <summary>
        /// Adds task to the ThreadPool queue for execution.
        /// </summary>
        private void QueueTask()
        {
            lock (_sync)
            {
                _jobsInQueue++;
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(RunTask));
        }

        /// <summary>
        /// Adds the <c>count</c> number of Task instances to the pool
        /// </summary>
        /// <param name="count">number of ITask instances to be added</param>
        private void AddTask(uint count)
        {
            lock (_sync)
            {
                for (int instanceIndex = 0;
                    instanceIndex < count
                        && _allTasks.Count < this.MaxInstances;
                    instanceIndex++)
                {
                    TaskWrapper task = new TaskWrapper(this.GetTaskInstance(), _sharedExecutionContext);
                    _allTasks.Add(task);
                    _freeTaskPool.Add(task);
                }
            }
        }
    }
}
