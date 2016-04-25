using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;

namespace XecMe.Core.Tasks
{
    public class RunOnceTaskRunner : TaskRunner
    {
        uint _delay;
        TaskWrapper _task;
        public RunOnceTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, uint delay, TraceType traceType)
            : base(name, taskType, parameters, traceType)
        {
            if (delay < 0)
                throw new ArgumentOutOfRangeException("delay", "delay cannot be negative");
            _delay = delay;
        }

        public override void Start()
        {
            if (_task == null)
            {
                _task = new TaskWrapper(this.GetTaskInstance(), new ExecutionContext(Parameters, this));
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                {
                    TraceInformation("will be run in {0}", TimeSpan.FromMilliseconds(_delay));
                    Thread.Sleep((int)_delay);
                    ExecutionState executionState = _task.RunTask();
                }));
            }
        }

        public override void Stop()
        {
            _task.Release();
            _task = null;
        }
    }
}
