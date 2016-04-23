#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TaskRunner.cs is part of XecMe.
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
using System.Collections.Specialized;
using XecMe.Core.Utils;
using XecMe.Common;
using XecMe.Core.Events;
using System.Threading;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// TaskRunner is the base class for all TaskRunners, this class will serve as base class for 
    /// all the 
    /// </summary>
    public abstract class TaskRunner
    {
        private Type _taskType;
        private StringDictionary _parameters;
        private EventWaitHandle _waitHandle;
        public TaskRunner(Type taskType, StringDictionary parameters)
        {
            Guard.ArgumentNotNull(taskType, "taskType");
            Guard.ArgumentNotNull(parameters, "parameters");
            _taskType = taskType;
            _parameters = parameters;
            _waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        protected StringDictionary Parameters
        {
            get { return _parameters; }
        }

        protected ITask GetTaskInstance()
        {
            ITask task = Reflection.CreateInstance<ITask>(_taskType);
            EventManager.Register(task);
            return task;
        }

        internal WaitHandle WaitHandle
        {
            get { return _waitHandle; }
        }

        public virtual void Start()
        {
            _waitHandle.Reset();
        }

        public virtual void Stop()
        {
            _waitHandle.Set();
        }
    }
}
