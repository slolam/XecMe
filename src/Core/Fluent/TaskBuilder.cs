using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    public interface ITaskBuilder
    {
        ITaskBuilder AddParameter(string name, object value);

        void Add();
    }
    internal abstract class TaskBuilder: ITaskBuilder
    {

        internal TaskBuilder(FlowConfiguration config, string name, Type taskType)
        {
            config.NotNull(nameof(config));
            name.NotNull(nameof(name));
            taskType.NotNull(nameof(taskType));
            
            if (taskType.IsAbstract || !typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException($"{taskType} is either abstract or not if type {typeof(ITask)}");
            }

            Config = config;
            Name = name;
            TaskType = taskType;
            Parameters = new Dictionary<string, object>();
        }

        protected FlowConfiguration Config { get; private set; }
        protected string Name { get; private set; }
        protected Type TaskType { get; private set; }
        protected TraceType TraceType { get; set; }
        protected Dictionary<string, object> Parameters { get; private set; }

        public ITaskBuilder AddParameter(string name, object value)
        {
            name.NotNull(nameof(name));
            value.NotNull(nameof(value));

            Parameters[name] = value;

            return this;
        }



        public abstract void Add();

    }
}
