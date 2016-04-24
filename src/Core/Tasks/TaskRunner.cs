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
using System.Text;
using System.Collections.Specialized;
using XecMe.Core.Utils;
using XecMe.Common;
using XecMe.Core.Events;
using System.Threading;
using System.Diagnostics;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Tasks
{
    public enum TraceType
    {
        None = 0,
        Error = 1,
        Warning = Error * 2,
        Information = Warning * 2,
        All = (Information * 2) - 1
    }
    /// <summary>
    /// TaskRunner is the base class for all TaskRunners, this class will serve as base class for 
    /// all the 
    /// </summary>
    public abstract class TaskRunner
    {
        private Type _taskType;
        private StringDictionary _parameters;
        private EventWaitHandle _waitHandle;
        private string _name;
        private bool isErrorTrace, isWarningTrace, isInfoTrace; 
        private string _taskRunnerTypeName;
        protected readonly Stopwatch _stopwatch = new Stopwatch();
        public event Events.EventHandler<ExecutionContext> Completed;
        public TaskRunner(string name, Type taskType, StringDictionary parameters, TraceType traceType)
        {
            name.NotNullOrEmpty(nameof(name));
            taskType.NotNull(nameof(taskType));
            parameters.NotNull(nameof(parameters));
            _name = name;
            _taskType = taskType;
            _parameters = parameters;
            isErrorTrace = (TraceType.Error & traceType) == TraceType.Error;
            isInfoTrace = (TraceType.Information & traceType) == TraceType.Information;
            isWarningTrace = (TraceType.Warning & traceType) == TraceType.Warning;
            _waitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            EventManager.AddPublisher(string.Concat("Task.", _name, ".Completed"), this, "Completed");
            _taskRunnerTypeName = this.GetType().Name;
        }

        internal Type TaskType
        {
            get { return _taskType; }
        }
        public string Name
        {
            get { return _name; }
        }

        protected StringDictionary Parameters
        {
            get { return _parameters; }
        }

        protected ITask GetTaskInstance()
        {
            //ITask task = Reflection.CreateInstance<ITask>(_taskType);
            ITask task = ExecutionContext.InternalContainer.GetInstance(_taskType) as ITask;
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

        protected void RaiseComplete(ExecutionContext context)
        {
            Completed(this, new EventArgs<ExecutionContext>(context.Copy()));
            TraceInformation("Task completed and raised the event");
            
        }

        
        protected internal void TraceInformation(string format, params object[] args)
        {
            if (isInfoTrace)
                Information(string.Format(string.Concat(_taskRunnerTypeName, " Task \"",_name,"\" : ",format),(object[]) args));
        }

        protected internal void TraceInformation(string msg)
        {
            if (isInfoTrace)
                Information(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", _name, "\" : ", msg)));
        }

        protected internal void TraceWarning(string format, params object[] args)
        {
            if (isInfoTrace)
                Warning(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", _name, "\" : ", format), (object[]) args));
        }

        protected internal void TraceWarning(string msg)
        {
            if (isWarningTrace)
                Warning(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", _name, "\" : ", msg)));
        }

        protected internal void TraceError(string format, params object[] args)
        {
            if (isErrorTrace)
                Error(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", _name, "\" : ", format), (object[])args));
        }

        protected internal void TraceError(string msg)
        {
            if (isErrorTrace)
                Error(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", _name, "\" : ", msg)));
        }

        private void Information(string log)
        {
            Log.Information(log);
        }

        private void Error(string log)
        {
            Log.Error(log);
        }
        private void Warning(string log)
        {
            Log.Warning(log);
        }
    }
}
