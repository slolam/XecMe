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
using System.Linq;
using System.Text;
using XecMe.Core.Utils;
using XecMe.Common;
using System.Diagnostics;

namespace XecMe.Core.Tasks
{
    internal class TaskWrapper
    {
        private ITask _task;
        private ExecutionContext _executionContext;
        private bool _initialized;
        public TaskWrapper(ITask task, ExecutionContext executionContext)
        {
            Guard.ArgumentNotNull(task, "task");
            Guard.ArgumentNotNull(executionContext, "executionContext");
            _executionContext = executionContext;
            _task = task;
        }

        public ExecutionState RunTask()
        {
            if (!_initialized)
            {
                try
                {
                    _task.OnStart(_executionContext);
                    _initialized = true;
                }
                catch (Exception e)
                {
                    try
                    {
                        _task.OnUnhandledException(e);
                    }
                    catch (Exception badEx)
                    {
                        Trace.TraceError("Bad Error: {0}", badEx);
                    }
                }
            }
            try
            {
                ExecutionState state = _task.OnExecute(_executionContext);
                if (state == ExecutionState.Recycle
                    || state == ExecutionState.Stop)
                {
                    _task.OnStop(_executionContext);
                    _initialized = false;
                }
                return state;
            }
            catch (Exception ex)
            {
                try
                {
                    _task.OnUnhandledException(ex);
                }
                catch (Exception badEx)
                {
                    Trace.TraceError("Bad Error: {0}", badEx);
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
            }
            catch (Exception e)
            {
                try
                {
                    _task.OnUnhandledException(e);
                }
                catch (Exception badEx)
                {
                    Trace.TraceError("Bad Error: {0}", badEx);
                }
            }
        }

        public ExecutionContext Context
        {
            get { return _executionContext; }
        }
    }
}
