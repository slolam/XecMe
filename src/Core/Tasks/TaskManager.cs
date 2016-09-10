#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TaskManager.cs is part of XecMe.
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
using System.Threading;
using XecMe.Core.Configuration;
using System.Diagnostics;
using XecMe.Common.Diagnostics;
using XecMe.Common.Injection;
using XecMe.Core.Injection;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Task manager to manage the task runners 
    /// </summary>
    public static class TaskManager
    {
        /// <summary>
        /// The task runners
        /// </summary>
        private static List<TaskRunner> _taskRunners;

        /// <summary>
        /// Initializes the <see cref="TaskManager"/> class.
        /// </summary>
        static TaskManager()
        {
            _taskRunners = new List<TaskRunner>();
        }

        /// <summary>
        /// Starts the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void Start(ITaskManagerConfig config)
        {
            IContainer container = new DefaultContainer();
            Start(config, container);
        }

        /// <summary>
        /// Starts the specified configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="container">The container.</param>
        public static void Start(ITaskManagerConfig config, IContainer container)
        {
            Stop();

            _taskRunners.AddRange(config.Runners);


            /////Play safe with the duplicate tasks
            //var orig = container.Options.AllowOverridingRegistrations;

            //container.Options.AllowOverridingRegistrations = true;

            for (int runnerIndex = 0; runnerIndex < _taskRunners.Count; runnerIndex++)
            {
                container.Register(_taskRunners[runnerIndex].TaskType);
            }

            if (Bootstrap != null)
                Bootstrap(container);


            /////Restore it back for the developer
            //container.Options.AllowOverridingRegistrations = orig;

            //try
            //{
            //    container.Verify();
            //}
            //catch(Exception e)
            //{
            //    Log.Error(string.Format("Error while verifying the container - {0}", e));
            //    throw;
            //}

            ExecutionContext.InternalContainer = container;

            for (int runnerIndex = 0; runnerIndex < _taskRunners.Count; runnerIndex++)
            {
                _taskRunners[runnerIndex].Start();
            }
            Log.Information("Task Manager started");
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public static void Stop()
        {
            while (_taskRunners.Count > 0)
            {
                _taskRunners[0].Stop();
                _taskRunners.RemoveAt(0);
            }
            Log.Information("Task Manager stopped");
        }

        /// <summary>
        /// Waits the tasks to complete.
        /// </summary>
        /// <param name="milliSeconds">The milli seconds.</param>
        public static void WaitTasksToComplete(int milliSeconds = -1)
        {
            if (_taskRunners.Count == 0)
                return;

            Log.Information(string.Format("Task Manager waiting for {0}", milliSeconds));

            WaitHandle[] handles = new WaitHandle[_taskRunners.Count];
            for (int i = 0; i < _taskRunners.Count; i++)
                handles[i] = _taskRunners[i].WaitHandle;

            if (milliSeconds < 0)
                WaitHandle.WaitAll(handles);
            else
                WaitHandle.WaitAll(handles, milliSeconds);
        }

        /// <summary>
        /// Gets or sets the bootstrap.
        /// </summary>
        /// <value>
        /// The bootstrap.
        /// </value>
        public static Action<IContainer> Bootstrap { get; set; }
    }
}
