using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    public interface IRunOnceTaskBuilder: ITaskBuilder
    {
        IRunOnceTaskBuilder Delay(uint delay);
    }
    internal class RunOnceTaskBuilder: TaskBuilder, IRunOnceTaskBuilder
    {
        uint _delay;
        internal RunOnceTaskBuilder(FlowConfiguration config, string name, Type taskType)
            :base(config, name, taskType)
        {

        }

        public IRunOnceTaskBuilder Delay(uint delay)
        {
            _delay = delay;
            return this;
        }

        public override void Add()
        {
            Config.InternalRunners.Add(new RunOnceTaskRunner(Name, TaskType, Parameters, _delay, TraceType));
        }
    }
}
