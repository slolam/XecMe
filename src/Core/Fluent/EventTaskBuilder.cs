using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Events;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    public interface IEventTaskBuilder: ITaskBuilder
    {
        IEventTaskBuilder SubscribeTopic(string topic);

        IEventTaskBuilder ThreadOption(ThreadOption threadOption);

        IEventTaskBuilder WaitBeforeExit(int timeout);
    }
    internal class EventTaskBuilder: TaskBuilder, IEventTaskBuilder
    {
        string _topic;
        ThreadOption _threadOption;
        int _timeout;
        internal EventTaskBuilder(FlowConfiguration config, string name, Type taskType)
            :base(config, name, taskType)
        {

        }

        public override void Add()
        {
            Config.InternalRunners.Add(new EventTaskRunner(Name, TaskType, Parameters, _topic, _threadOption, _timeout, TraceType));
        }

        public IEventTaskBuilder SubscribeTopic(string topic)
        {
            _topic = topic;
            return this;
        }

        public IEventTaskBuilder ThreadOption(ThreadOption threadOption)
        {
            _threadOption = threadOption;
            return this;
        }

        public IEventTaskBuilder WaitBeforeExit(int timeout)
        {
            _timeout = timeout;
            return this;
        }
    }
}
