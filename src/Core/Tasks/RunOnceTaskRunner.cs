using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace XecMe.Core.Tasks
{
    public class RunOnceTaskRunner : TaskRunner
    {
        uint _delay;
        TaskWrapper _taskWrapper;
        public RunOnceTaskRunner(string name, Type taskType, Dictionary<string, object> parameters, uint delay, LogType traceType)
            : base(name, taskType, parameters, traceType)
        {
            if (delay < 0)
                throw new ArgumentOutOfRangeException("delay", "delay cannot be negative");
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

        public override void Stop()
        {
            _taskWrapper.Release();
            _taskWrapper = null;
        }
    }
}
