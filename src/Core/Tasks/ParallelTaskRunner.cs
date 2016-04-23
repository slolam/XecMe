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
using System.Linq;
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
        private int _jobsInQueue;
        /// <summary>
        /// Minimum no. of tasks that should run in parallel
        /// </summary>
        /// <remarks>Although this is the configuration, the actual no. of tasks running 
        /// depends on lot of other factors</remarks>
        private int _minInstances;
        /// <summary>
        /// Maximum no. of tasks that should run in parallel
        /// </summary>
        /// <remarks>Although this is the configuration, the actual no. of tasks running 
        /// depends on lot of other factors</remarks>
        private int _maxInstances;
        /// <summary>
        /// This is the time in milliseconds the thread will sleep when there is no work
        /// </summary>
        private long _idlePollingPeriod;
        /// <summary>
        /// This indicate the total no. of task actually running in parallel at a given instance.
        /// </summary>
        private int _parallelInstances;
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

        public ParallelTaskRunner(string name, Type taskType, StringDictionary parameters, int min, int max, long idlePollingPeriod) :
            base(name, taskType, parameters)
        {
            if (min < 1 || max < min)
                throw new ArgumentOutOfRangeException("min", "min and max should be greater than 1 and min should be less than or equal to max");
            if (idlePollingPeriod < 100)
                throw new ArgumentOutOfRangeException("max", "idlePollingPeriod should be at least 100ms.");
            _minInstances = min;
            _maxInstances = max;
            _idlePollingPeriod = idlePollingPeriod;
            _jobsInQueue = 0;
        }

        /// <summary>
        /// Idle polling period in millisecond, this is the time the thread will sleep before attempting next time.
        /// </summary>
        /// <remarks>This is to reduce the load on the process when there is no work</remarks>
        public long IdlePollingPeriod
        {
            get { return _idlePollingPeriod; }
        }

        /// <summary>
        /// This is the minimum number of instances that will work in parallel
        /// </summary>
        public int MinInstances
        {
            get { return _minInstances; }
        }

        /// <summary>
        /// This is the maximun number of instance that will work in parallel in case of full load.
        /// </summary>
        /// <remarks>Although this defines the maximum parallel instance, but it depends on other factors
        /// like load, cpu utilization, cpu allocation to the process etc.</remarks>
        public int MaxInstances
        {
            get { return _maxInstances; }
        }

        /// <summary>
        /// Gets the number of instances that are running in parallel.
        /// </summary>
        public int ParallelInstances
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
                    Trace.TraceInformation("Parallel task \"{0}\" has started", this.Name);
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
                    Trace.TraceInformation("Parallel task \"{0}\" has stopped", this.Name);
                }
            }
        }

        #endregion

        private void RunTask(object state)
        {
            if (!_isStarted)
                return;

            TaskWrapper taskWrapper = null;
            lock (_sync)
            {
                // if this is coming as part of the timer trigger close down the timer
                if (_timer != null)
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);

                _jobsInQueue--;

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
                    return;
                }
                _parallelInstances++;
            }

            Trace.TraceInformation("Parallel task \"{0}\" is running {1} threads", this.Name, _parallelInstances);

            ExecutionState executionState = taskWrapper.RunTask();
            Trace.TraceInformation("Parallel task \"{0}\" has executed with return value {1}", this.Name, executionState);
    
            switch (executionState)
            {
                case ExecutionState.Executed:
                    //Since there is work it should also create one more instance for parallel tasking
                    lock (_sync)
                    {
                        ///Put back the task into the free pool
                        _freeTaskPool.Add(taskWrapper);
                        //Since executed successfully it should go back for execution
                        QueueTask();
                        //there is more capacity possible, put one more to work
                        if (_jobsInQueue < this.MaxInstances)
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
                        _allTasks.Remove(taskWrapper);
                        _freeTaskPool.Remove(taskWrapper);
                        if (_jobsInQueue == 0 && _parallelInstances == 1)
                            Stop();
                    }
                    break;
                case ExecutionState.Recycle:
                    //Since the task has indicated to recycle it, release it and add new.
                    lock (_sync)
                    {
                        _allTasks.Remove(taskWrapper);
                        _freeTaskPool.Remove(taskWrapper);
                        AddTask(1);
                        QueueTask();
                    }
                    break;
                case ExecutionState.Idle:
                    //This is tough task :-), not enough work to do, start firing ;-)
                    lock (_sync)
                    {
                        ///Put back the task into the free pool
                        _freeTaskPool.Add(taskWrapper);
                        //There are other items in the queue do nothing come out.
                        if (_jobsInQueue > 0)
                            break;

                        ///Fire 1 task, save resources
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
                        ///Wait for 5 second to see whether there is something to do
                        _jobsInQueue++;
                        _timer.Change(_idlePollingPeriod, Timeout.Infinite);
                    }
                    break;
                default:
                    lock (_sync)
                    {
                        ///Put back the task into the free pool
                        _freeTaskPool.Add(taskWrapper);
                        QueueTask();
                    }
                    break;
            }

            lock (_sync)
            {
                _parallelInstances--;
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
        private void AddTask(int count)
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
