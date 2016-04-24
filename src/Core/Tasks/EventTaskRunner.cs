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
using System.Text;
using System.Collections.Specialized;
using XecMe.Core.Events;
using XecMe.Common;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// EventTaskRunner subscribe to a event topic and runs the task when it receive the event in the topic
    /// </summary>
    public class EventTaskRunner : TaskRunner
    {
        /// <summary>
        /// Instance of the task
        /// </summary>
        private ITask _task;
        /// <summary>
        /// Execution context for the task
        /// </summary>
        private ExecutionContext _executionContext;
        /// <summary>
        /// Name of the event topic this runner is subscribed to
        /// </summary>
        private string _eventTopic;
        /// <summary>
        /// Threading option fo this task runner
        /// </summary>
        private ThreadOption _threadOption;
        /// <summary>
        /// Mutext for synchronization
        /// </summary>
        private ManualResetEvent _syncEvent = null;
        /// <summary>
        /// Event arguments queued for execution
        /// </summary>
        private Queue<EventArgs> _queue = null;
        /// <summary>
        /// Indicate whether a thread is processing
        /// </summary>
        private bool _threadWorking = false;
        /// <summary>
        /// Timeout before the task stops
        /// </summary>
        private int _timeout;
        /// <summary>
        /// Contructor for the EentTaskRunner
        /// </summary>
        /// <param name="name">Unique name of the task runner</param>
        /// <param name="taskType">.NET Type for the task</param>
        /// <param name="parameters">Parameters from the configuration</param>
        /// <param name="eventTopic">Name of the event topic</param>
        /// <param name="threadOption">ThreadOption for the task</param>
        /// <param name="timeout">Timeout of the runner before it stops</param>
        /// <param name="traceType">TraceType filter for this runner</param>
        public EventTaskRunner(string name, Type taskType, StringDictionary parameters, string eventTopic, ThreadOption threadOption, int timeout, TraceType traceType) :
            base(name, taskType, parameters, traceType)
        {
            eventTopic.NotNullOrEmpty(nameof(eventTopic));
            if (timeout < 1 || timeout > 10000)
                throw new ArgumentOutOfRangeException("timeout", "timeout should be between 1 and 10,000 ms (10 sec)");

            _timeout = timeout;
            _eventTopic = eventTopic;
            _threadOption = threadOption;
            _executionContext = new ExecutionContext(this.Parameters, this);

            if (threadOption != ThreadOption.BackgroundSerial)
                _syncEvent = new ManualResetEvent(true);

            if (threadOption == ThreadOption.BackgroundSerial)
                _queue = new Queue<EventArgs>();
        }


        #region TaskRunner Members
        /// <summary>
        /// Starts this task runner
        /// </summary>
        public override void Start()
        {
            lock (this)
            {
                if (_task == null)
                    _task = this.GetTaskInstance();
            }

            EventManager.AddSubscriber(_eventTopic, this, "EventSink", _threadOption);
            if (_threadOption != ThreadOption.Publisher)
                base.Start();
            _task.OnStart(_executionContext);
            TraceInformation("Started");
        }

        /// <summary>
        /// Stops this task runner
        /// </summary>
        public override void Stop()
        {
            EventManager.RemoveSubscriber(_eventTopic, this, "EventSink");
            if (_threadOption != ThreadOption.Publisher)
            {
                ThreadPool.QueueUserWorkItem(delegate(object o)
                {
                    Thread.Sleep(_timeout);
                    try
                    {
                        _task.OnStop(_executionContext);
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            TraceError("Caught unhandled exception calling OnStop, calling OnUnhandledException: {0}", e);
                            _task.OnUnhandledException(e);
                        }
                        catch (Exception badEx)
                        {
                            TraceError("Bad Error: {0}", badEx);
                        }
                    }
                    TraceInformation("Stopped");
                    base.Stop();
                });
            }
            else
            {
                try
                {
                    _task.OnStop(_executionContext);
                }
                catch (Exception e)
                {
                    try
                    {
                        TraceError("Caught unhandled exception calling OnStop, calling OnUnhandledException: {0}", e);
                        _task.OnUnhandledException(e);
                    }
                    catch (Exception badEx)
                    {
                        TraceError("Bad Error: {0}", badEx);
                    }
                }
                TraceInformation("Stopped", this.Name);
            }
        }

        #endregion

        #region Event Subscription
        /// <summary>
        /// This method subscribes to the event
        /// </summary>
        /// <param name="sender">Publisher object</param>
        /// <param name="args">EventArg of published by publisher</param>
        private void EventSink(object sender, EventArgs args)
        {
            switch (_threadOption)
            {
                    ///Runs the events on the backgroud thread but in the order the events are received one event at a time
                case ThreadOption.BackgroundSerial:
                    lock (_queue)
                    {
                        ///Add the event to the queue
                        _queue.Enqueue(args);

                        ///If the thread is already processing the return
                        if (_threadWorking)
                        {
                            return;
                        }

                        ///Indicate that this thread will process the events
                        _threadWorking = true;
                    }

                    ///While there are items in the queue process
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
                                ///If there are no items in the queue mark that the thread is released and exit
                                _threadWorking = false;
                                break;
                            }
                        }
                        RunTask(args);
                    }
                    break;
                default:///Rest, Background Parallel and Publisher runs in a free threading more
                    RunTask(args);
                    break;
            }

        }

        /// <summary>
        /// Runs the underlying task
        /// </summary>
        /// <param name="args">EventArg published by publisher</param>
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
                        ec["EventArgs"] = argType.InvokeMember("Value", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public, null, args, null);
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
                TraceInformation("Is executing on Managed thread {0}",Thread.CurrentThread.ManagedThreadId);
                _stopwatch.Restart();
                ExecutionState state = _task.OnExecute(ec);
                _stopwatch.Stop();
                TraceInformation("Executed in {2} ms. with a return value {0} on Managed thread {1}", state, Thread.CurrentThread.ManagedThreadId, _stopwatch.ElapsedMilliseconds);
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
                                        TraceError("Caught unhandled exception calling OnStop, calling OnUnhandledException: {0}", e);
                                        _task.OnUnhandledException(e);
                                    }
                                    catch (Exception badEx)
                                    {
                                        TraceError("Bad Error: {0}", badEx);
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
                _stopwatch.Stop();
                TraceInformation("Executed in {2} ms. with a return value {0} on Managed thread {1}", ExecutionState.Exception, Thread.CurrentThread.ManagedThreadId, _stopwatch.ElapsedMilliseconds);
                try
                {
                    TraceError("Caught unhandled exception calling OnExecute, calling OnUnhandledException: {0}", ex);
                    _task.OnUnhandledException(ex);
                }
                catch (Exception badEx)
                {
                    TraceError("Bad Error: {0}", badEx);
                }
            }
        }

        #endregion
        /// <summary>
        /// Sets the mutex to allow thread to process
        /// </summary>
        private void Set()
        {
            if (_threadOption != ThreadOption.BackgroundSerial)
                _syncEvent.Set();
        }

        /// <summary>
        /// Resets the mutex to block the threads to process
        /// </summary>
        private void Reset()
        {
            if (_threadOption != ThreadOption.BackgroundSerial)
                _syncEvent.Reset();
        }

        /// <summary>
        /// Waits until mutex is signaled
        /// </summary>
        private void Wait()
        {
            if (_threadOption != ThreadOption.BackgroundSerial)
                _syncEvent.WaitOne();
        }
    }
}
