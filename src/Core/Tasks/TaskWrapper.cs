#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TaskWrapper.cs is part of XecMe.
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
using XecMe.Core.Utils;
using XecMe.Common;
using System.Diagnostics;
using System.Threading;

namespace XecMe.Core.Tasks
{
    internal class TaskWrapper
    {
        private ITask _task;
        private ExecutionContext _executionContext;
        private bool _initialized;
        private Stopwatch _stopwatch = new Stopwatch();
        public TaskWrapper(ITask task, ExecutionContext executionContext)
        {
            task.NotNull(nameof(task));
            executionContext.NotNull(nameof(executionContext));
            _executionContext = executionContext;
            _task = task;
        }

        public ITask TaskType
        {
            get
            {
                return _task;
            }
        }

        public ExecutionState RunTask()
        {
            if (!_initialized)
            {
                try
                {
                    _task.OnStart(_executionContext);
                    _initialized = true;
                    _executionContext.TaskRunner.TraceInformation("Started");
                }
                catch (Exception e)
                {
                    try
                    {
                        _executionContext.TaskRunner.TraceError("Caught unhandled exception in OnStart, calling OnUnhandledException: {0}", e);
                        _task.OnUnhandledException(e);
                    }
                    catch (Exception badEx)
                    {
                        _executionContext.TaskRunner.TraceError("Bad Error: {0}", badEx);
                    }
                }
            }
            try
            {
                _executionContext.TaskRunner.TraceInformation("Executing on Managed thread {0}",Thread.CurrentThread.ManagedThreadId);
                _stopwatch.Restart();
                ExecutionState state = _task.OnExecute(_executionContext);
                _stopwatch.Stop();
                _executionContext.TaskRunner.TraceInformation("Executed in {2} ms. with a return value {0} on Managed thread {1}", state, Thread.CurrentThread.ManagedThreadId, _stopwatch.ElapsedMilliseconds);
                if (state == ExecutionState.Recycle
                    || state == ExecutionState.Stop)
                {
                    Release();
                }
                return state;
            }
            catch (Exception ex)
            {
                _stopwatch.Stop();
                _executionContext.TaskRunner.TraceInformation("Executed in {2} ms. with a return value {0} on Managed thread {1}", ExecutionState.Exception, Thread.CurrentThread.ManagedThreadId, _stopwatch.ElapsedMilliseconds);
                try
                {
                    _executionContext.TaskRunner.TraceError("Caught unhandled exception in OnExecute, calling OnUnhandledException: {0}", ex);
                    _task.OnUnhandledException(ex);
                }
                catch (Exception badEx)
                {
                    _executionContext.TaskRunner.TraceError("Bad Error: {0}", badEx);
                }
                return ExecutionState.Exception;
            }
        }

        public void Release()
        {
            try
            {
                if (_initialized)
                    _task.OnStop(_executionContext);
                _executionContext.TaskRunner.TraceInformation("Stopped successfully");
            }
            catch (Exception e)
            {
                try
                {
                    _executionContext.TaskRunner.TraceError("Caught unhandled exception in OnStop, calling OnUnhandledException: {0}", e);
                    _task.OnUnhandledException(e);
                }
                catch (Exception badEx)
                {
                    _executionContext.TaskRunner.TraceError("Bad Error: {0}", badEx);
                }
            }
            _initialized = false;
        }

        public ExecutionContext Context
        {
            get { return _executionContext; }
        }
    }
}
