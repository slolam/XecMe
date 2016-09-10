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
    /// <summary>
    /// Log type of the logging
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// No logging at all
        /// </summary>
        None = 0,
        /// <summary>
        /// Log only errors
        /// </summary>
        Error = 1,
        /// <summary>
        /// Log errors and warnings
        /// </summary>
        Warning = Error * 2,
        /// <summary>
        /// Log informations, warnings and errors
        /// </summary>
        Information = Warning * 2,
        /// <summary>
        /// Log everything
        /// </summary>
        All = (Information * 2) - 1
    }
    /// <summary>
    /// TaskRunner is the base class for all TaskRunners, this class will serve as base class for 
    /// all the concrete runner implementations
    /// </summary>
    public abstract class TaskRunner
    {
        //private Type _taskType;
        //private TaskWrapper _taskWrapper;        
        /// <summary>
        /// The is error trace
        /// </summary>
        private bool isErrorTrace, isWarningTrace, isInfoTrace;
        /// <summary>
        /// The task runner type name
        /// </summary>
        private string _taskRunnerTypeName;
        /// <summary>
        /// The stopwatch
        /// </summary>
        protected readonly Stopwatch _stopwatch = new Stopwatch();
        /// <summary>
        /// Occurs when [completed].
        /// </summary>
        public event Events.EventHandler<ExecutionContext> Completed;

        /// <summary>
        /// Initializes the task runner
        /// </summary>
        /// <param name="name"></param>
        /// <param name="taskType"></param>
        /// <param name="parameters"></param>
        /// <param name="logType"></param>
        public TaskRunner(string name, Type taskType, Dictionary<string, object> parameters, LogType logType)
        {
            name.NotNullOrEmpty(nameof(name));
            taskType.NotNull(nameof(taskType));
            parameters.NotNull(nameof(parameters));

            if(!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException($"Type {taskType} does not implement the type {typeof(ITask)}", nameof(taskType));
            }

            if(taskType.IsAbstract)
            {
                throw new ArgumentException($"Type {taskType} cannot be an abstract type", nameof(taskType));
            }

            Name = name;
            TaskType = taskType;
            Parameters = parameters;
            isErrorTrace = (LogType.Error & logType) == LogType.Error;
            isInfoTrace = (LogType.Information & logType) == LogType.Information;
            isWarningTrace = (LogType.Warning & logType) == LogType.Warning;
            WaitHandle = new EventWaitHandle(true, EventResetMode.ManualReset);
            EventManager.AddPublisher(string.Concat("Task.", name, ".Completed"), this, "Completed");
            _taskRunnerTypeName = this.GetType().Name;
        }

        /// <summary>
        /// Task type
        /// </summary>
        internal Type TaskType { get; private set;  }

        /// <summary>
        /// Name of the task
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the parameters
        /// </summary>
        protected Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Gets the task instance.
        /// </summary>
        /// <returns></returns>
        protected ITask GetTaskInstance()
        {
            ITask task = ExecutionContext.InternalContainer.GetInstance(TaskType) as ITask;
            EventManager.Register(task);
            return task;
        }

        /// <summary>
        /// Wait handle for this task runner
        /// </summary>
        internal WaitHandle WaitHandle { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public virtual void Start()
        {
            ((EventWaitHandle)WaitHandle).Reset();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public virtual void Stop()
        {
            ((EventWaitHandle)WaitHandle).Set();
        }

        /// <summary>
        /// Raises the complete.
        /// </summary>
        /// <param name="context">The context.</param>
        protected void RaiseComplete(ExecutionContext context)
        {
            Completed(this, new EventArgs<ExecutionContext>(context.Copy()));
            TraceInformation($"{Name} task completed and raised the event");
        }

        /// <summary>
        /// Traces the information.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        protected internal void TraceInformation(string format, params object[] args)
        {
            if (isInfoTrace)
            {
                Information(string.Format(string.Concat(_taskRunnerTypeName, " Task \"",Name,"\" : ",format),(object[]) args));
            }
        }

        /// <summary>
        /// Traces the information.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        protected internal void TraceInformation(string msg)
        {
            if (isInfoTrace)
            {
                Information(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", Name, "\" : ", msg)));
            }
        }

        /// <summary>
        /// Traces the warning.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        protected internal void TraceWarning(string format, params object[] args)
        {
            if (isInfoTrace)
            {
                Warning(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", Name, "\" : ", format), (object[])args));
            }
        }

        /// <summary>
        /// Traces the warning.
        /// </summary>
        /// <param name="msg">The message.</param>
        protected internal void TraceWarning(string msg)
        {
            if (isWarningTrace)
            {
                Warning(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", Name, "\" : ", msg)));
            }
        }

        /// <summary>
        /// Traces the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        protected internal void TraceError(string format, params object[] args)
        {
            if (isErrorTrace)
            {
                Error(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", Name, "\" : ", format), (object[])args));
            }
        }

        /// <summary>
        /// Traces the error.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        protected internal void TraceError(string msg)
        {
            if (isErrorTrace)
            {
                Error(string.Format(string.Concat(_taskRunnerTypeName, " Task \"", Name, "\" : ", msg)));
            }
        }

        /// <summary>
        /// Informations the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        private void Information(string log)
        {
            Log.Information(log);
        }

        /// <summary>
        /// Errors the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        private void Error(string log)
        {
            Log.Error(log);
        }

        /// <summary>
        /// Warnings the specified log.
        /// </summary>
        /// <param name="log">The log.</param>
        private void Warning(string log)
        {
            Log.Warning(log);
        }
    }
}
