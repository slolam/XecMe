using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Threading;

namespace XecMe.Core.Tasks
{
    public class RunOnceTaskRunner : TaskRunner
    {
        int _delay;
        TaskWrapper _task;
        public RunOnceTaskRunner(string name, Type taskType, StringDictionary parameters, int delay)
            : base(name, taskType, parameters)
        {
            if (delay < 0)
                throw new ArgumentOutOfRangeException("delay", "delay cannot be negative");
            _delay = delay;
        }

        public override void Start()
        {
            if (_task == null)
            {
                _task = new TaskWrapper(this.GetTaskInstance(), new ExecutionContext(Parameters));
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object state)
                {
                    Thread.Sleep(_delay);
                    _task.RunTask();
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
