#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file EventTaskRunner.cs is part of XecMe.
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
using XecMe.Core.Events;
using XecMe.Common;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace XecMe.Core.Tasks
{
    public class EventTaskRunner : TaskRunner
    {
        private ITask _task;
        private ExecutionContext _executionContext;
        private string _eventName;
        private ThreadOption _threadOption;
        private Type _eventType;
        public EventTaskRunner(Type taskType, StringDictionary parameters, string eventName, ThreadOption threadOption) :
            base(taskType, parameters)
        {
            Guard.ArgumentNotNullOrEmptyString(eventName, "eventName");
            _eventName = eventName;
            _threadOption = threadOption;
            _executionContext = new ExecutionContext(this.Parameters, this);
            _task = this.GetTaskInstance();
        }


        #region TaskRunner Members

        public override void Start()
        {
            EventManager.AddSubscriber(_eventName, this, "EventSink", _threadOption);
            _task.OnStart(_executionContext);
        }

        public override void Stop()
        {
            EventManager.RemoveSubscriber(_eventName, this, "EventSink");
            _task.OnStop(_executionContext);

        }

        #endregion

        #region Event Subscription
        private void EventSink(object sender, EventArgs args)
        {
            ExecutionContext ec = _executionContext.Copy();
            ec["EventArgs"] = args;
            try
            {
                ExecutionState state = _task.OnExecute(ec);
                switch (state)
                {
                    case ExecutionState.Stop:
                        ///Stop listening to the event in future
                        this.Stop();
                        break;
                    case ExecutionState.Recycle:
                        try
                        {
                            _task.OnStop(ec);
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                _task.OnUnhandledException(e);
                            }
                            catch (Exception badEx)
                            {
                                Trace.TraceError("Bad Error:" + badEx.ToString());
                            }
                        }
                        _task = this.GetTaskInstance();
                        break;
                }
            }
            catch (Exception ex)
            {
                _task.OnUnhandledException(ex);
            }
        }

        #endregion

    }
}
