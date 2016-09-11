using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using XecMe.Core.Events;
using XecMe.Common;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;

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
        //private ITask _task;
        /// <summary>
        /// The task wrapper
        /// </summary>
        private TaskWrapper _taskWrapper;
        /// <summary>
        /// Execution context for the task
        /// </summary>
        //private ExecutionContext _executionContext;
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
        private Queue<ExecutionContext> _queue = null;
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
        /// <param name="logType">TraceType filter for this runner</param>
        public EventTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, string eventTopic, ThreadOption threadOption, int timeout, LogType logType) :
            base(name, taskType, parameters, logType)
        {
            eventTopic.NotNullOrEmpty(nameof(eventTopic));
            if (timeout < 1 || timeout > 10000)
                throw new ArgumentOutOfRangeException("timeout", "timeout should be between 1 and 10,000 ms (10 sec)");

            _timeout = timeout;
            _eventTopic = eventTopic;
            _threadOption = threadOption;
            _taskWrapper = new TaskWrapper(taskType, new ExecutionContext(this.Parameters, this));

            if (threadOption == ThreadOption.BackgroundSerial)
            {
                _queue = new Queue<ExecutionContext>();
            }
            else
            {
                _syncEvent = new ManualResetEvent(true);
            }
        }

        #region TaskRunner Members
        /// <summary>
        /// Starts this task runner
        /// </summary>
        public override void Start()
        {
            EventManager.AddSubscriber(_eventTopic, this, "EventSink", _threadOption);
            if (_threadOption != ThreadOption.Publisher)
                base.Start();
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
                ThreadPool.QueueUserWorkItem(async (o) =>
                {
                    await Task.Delay(_timeout);
                    _taskWrapper.Release();
                    TraceInformation("Stopped");
                    base.Stop();
                });
            }
            else
            {
                _taskWrapper.Release();
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
                    ec = _taskWrapper.Context.Copy();
                    if (argType.GetGenericTypeDefinition() == typeof(EventArgs<>))
                    {
                        ec.EventArg = argType.InvokeMember("Value", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public, null, args, null);
                    }
                    else
                    {
                        ec.EventArg = args;
                    }
                }
            }
            ec.TaskRunner = this;

            switch (_threadOption)
            {
                ///Runs the events on the backgroud thread but in the order the events are received one event at a time
                case ThreadOption.BackgroundSerial:
                    lock (_queue)
                    {
                        ///Add the event to the queue
                        _queue.Enqueue(ec);

                        ///If the thread is already processing then return
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
                                ec = _queue.Dequeue();
                            }
                            else
                            {
                                ///If there are no items in the queue mark that the thread is released and exit
                                _threadWorking = false;
                                break;
                            }
                        }
                        RunTask(ec);
                    }
                    break;
                default:///Rest, Background Parallel and Publisher runs in a free threading more
                    RunTask(ec);
                    break;
            }

        }

        /// <summary>
        /// Runs the underlying task
        /// </summary>
        /// <param name="args">EventArg published by publisher</param>
        private void RunTask(ExecutionContext ec)
        {
            this.Wait();
            // Hold the reference of the current task instance
            ITask task = _taskWrapper.Task;
            ExecutionState state = _taskWrapper.RunTask(ec);
            switch (state)
            {
                case ExecutionState.Stop:
                    ///Stop listening to the event in future
                    _taskWrapper.Release(ec);
                    this.Stop();
                    break;
                case ExecutionState.Recycle:
                    Reset();
                    lock (this)
                    {
                        ///Check if no other thread already recycled the task
                        if (ReferenceEquals(task, _taskWrapper.Task))
                        {
                            _taskWrapper.Release(ec);
                        }
                    }
                    Set();
                    ///Since this is Event based we will end up losing the event
                    ///hence call the task again, however you don't really manage the recycling again
                    //RunTask(ec);
                    state = _taskWrapper.RunTask(ec);
                    break;
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
