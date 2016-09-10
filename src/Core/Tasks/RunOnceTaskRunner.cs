using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Runner to run the task once
    /// </summary>
    /// <seealso cref="XecMe.Core.Tasks.TaskRunner" />
    public class RunOnceTaskRunner : TaskRunner
    {
        /// <summary>
        /// The delay before task is kicked off
        /// </summary>
        uint _delay;
        /// <summary>
        /// The task wrapper
        /// </summary>
        TaskWrapper _taskWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunOnceTaskRunner" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="delay">The delay.</param>
        /// <param name="traceType">Type of the trace.</param>
        /// <exception cref="ArgumentOutOfRangeException">Delay cannot be negative</exception>
        public RunOnceTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, uint delay, LogType traceType)
            : base(name, taskType, parameters, traceType)
        {
            if (delay < 0)
                throw new ArgumentOutOfRangeException(nameof(delay), "Delay cannot be negative");
            _delay = delay;
        }

        public override void Start()
        {
            if (_taskWrapper == null)
            {
                _taskWrapper = new TaskWrapper(this.TaskType, new ExecutionContext(Parameters, this));
                ThreadPool.QueueUserWorkItem(new WaitCallback(async (state) =>
                {
                    TraceInformation("Will be executed in {0}", TimeSpan.FromMilliseconds(_delay));
                    await Task.Delay((int)_delay);
                    ExecutionState executionState = _taskWrapper.RunTask();
                }));
            }
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public override void Stop()
        {
            _taskWrapper.Release();
            _taskWrapper = null;
        }
    }
}
