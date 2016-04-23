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
        private string _eventTopic;
        private ThreadOption _threadOption;
        private ManualResetEvent _syncEvent = null;
        private Queue<EventArgs> _queue = null;
        private bool _threadWorking = false;
        public EventTaskRunner(string name, Type taskType, StringDictionary parameters, string eventTopic, ThreadOption threadOption) :
            base(name, taskType, parameters)
        {
            Guard.ArgumentNotNullOrEmptyString(eventTopic, "eventTopic");
            _eventTopic = eventTopic;
            _threadOption = threadOption;
            _executionContext = new ExecutionContext(this.Parameters, this);
            _task = this.GetTaskInstance();
            
            if (threadOption != ThreadOption.BackgroundSerial)
                _syncEvent = new ManualResetEvent(true);

            if (threadOption == ThreadOption.BackgroundSerial)
                _queue = new Queue<EventArgs>();
        }


        #region TaskRunner Members

        public override void Start()
        {
            EventManager.AddSubscriber(_eventTopic, this, "EventSink", _threadOption);
            _task.OnStart(_executionContext);
            Trace.TraceInformation("Event task \"{0}\" started", this.Name);
        }

        public override void Stop()
        {
            EventManager.RemoveSubscriber(_eventTopic, this, "EventSink");
            _task.OnStop(_executionContext);
            Trace.TraceInformation("Event task \"{0}\" stopped", this.Name);
        }

        #endregion

        #region Event Subscription
        private void EventSink(object sender, EventArgs args)
        {
            switch (_threadOption)
            {
                case ThreadOption.BackgroundSerial:
                    lock (_queue)
                    {
                        _queue.Enqueue(args);

                        if (_threadWorking)
                        {
                            return;
                        }

                        _threadWorking = true;
                    }

                    while (true)
                    {
                        lock (_queue)
                        {
                            if (_queue.Count > 0)
                            {
                                args = _queue.Dequeue();
                            }
                            else
                            {
                                _threadWorking = false;
                                break;
                            }
                        }
                        RunTask(args);
                    }
                    break;
                default:
                    RunTask(args);
                    break;
            }

        }
        private void RunTask(EventArgs args)
        {
            ExecutionContext ec = null;
            
            if (args is EventArgs<ExecutionContext>)
            {
                ec = ((EventArgs<ExecutionContext>)args).Value;
            }
            else 
            {
                Type argType = args.GetType();
                if (argType.IsGenericType)
                {
                    ec = _executionContext.Copy();
                    if (argType.GetGenericTypeDefinition() == typeof(EventArgs<>))
                    {
                        ec["EventArgs"] = argType.InvokeMember("get_Value", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public, null, args, null);
                    }
                    else
                    {
                        ec["EventArgs"] = args;
                    }
                }
            }
            ec.TaskRunner = this;

            try
            {
                this.Wait();
                ITask task = _task;
                ExecutionState state = _task.OnExecute(ec);
                Trace.TraceInformation("Event task \"{0}\" is executed with a return value {1}", this.Name, state);
                switch (state)
                {
                    case ExecutionState.Stop:
                        ///Stop listening to the event in future
                        this.Stop();
                        break;
                    case ExecutionState.Recycle:
                        Reset();
                        lock (this)
                        {
                            ///Check if some other thread already recycled the task
                            if (task == _task)
                            {
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
                                        Trace.TraceError("Bad Error: {0}", badEx);
                                    }
                                }
                                _task = this.GetTaskInstance();
                            }
                        }
                        Set();
                        ///Since this is Event based we will end up losing the event
                        ///hence call the task again
                        RunTask(args);
                        break;
                }
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
            }
        }

        #endregion

        private void Set()
        {
            if (_threadOption != ThreadOption.BackgroundSerial)
                _syncEvent.Set();
        }

        private void Reset()
        {
            if (_threadOption != ThreadOption.BackgroundSerial)
                _syncEvent.Reset();
        }

        private void Wait()
        {
            if (_threadOption != ThreadOption.BackgroundSerial)
                _syncEvent.WaitOne();
        }
    }
}
