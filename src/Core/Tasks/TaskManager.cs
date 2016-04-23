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
using SimpleInjector;

namespace XecMe.Core.Tasks
{
    public static class TaskManager
    {
        private static List<TaskRunner> _taskRunners;

        static TaskManager()
        {
            _taskRunners = new List<TaskRunner>();
        }

        public static void Start(ITaskManagerConfig config)
        {
            Stop();

            _taskRunners.AddRange(config.Runners);

            Container container = new Container();

            ///Play safe with the duplicate tasks
            container.Options.AllowOverridingRegistrations = true;

            for (int runnerIndex = 0; runnerIndex < _taskRunners.Count; runnerIndex++)
            {
                container.Register(_taskRunners[runnerIndex].TaskType);
            }
            ///Restore it back for the developer
            container.Options.AllowOverridingRegistrations = false;


            Bootstrap(container);

            ExecutionContext.InternalContainer = container;

            for (int runnerIndex = 0; runnerIndex < _taskRunners.Count; runnerIndex++)
            {
                _taskRunners[runnerIndex].Start();
            }
            Trace.TraceInformation("Task Manager started");
        }

        public static void Stop()
        {
            while (_taskRunners.Count > 0)
            {
                _taskRunners[0].Stop();
                _taskRunners.RemoveAt(0);
            }
            Trace.TraceInformation("Task Manager stopped");
        }

        public static void WaitTasksToComplete(int milliSeconds = -1)
        {
            if (_taskRunners.Count == 0)
                return;

            Trace.TraceInformation("Task Manager waiting for {0}", milliSeconds);

            WaitHandle[] handles = new WaitHandle[_taskRunners.Count];
            for (int i = 0; i < _taskRunners.Count; i++)
                handles[i] = _taskRunners[i].WaitHandle;

            if (milliSeconds < 0)
                WaitHandle.WaitAll(handles);
            else
                WaitHandle.WaitAll(handles, milliSeconds);
        }

        public static Action<Container> Bootstrap
        { get; set; }

    }
}
