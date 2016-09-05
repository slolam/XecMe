using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    /// <summary>
    /// Base task builder interface 
    /// </summary>
    public interface ITaskBuilder
    {
        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        ITaskBuilder AddParameter(string name, object value);

        /// <summary>
        /// Adds the task to the configuration
        /// </summary>
        void Add();
    }

    /// <summary>
    /// Base task builder for fluent APIs
    /// </summary>
    /// <seealso cref="ITaskBuilder" />
    internal abstract class TaskBuilder: ITaskBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskBuilder"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="logType">Type of the log.</param>
        /// <exception cref="System.ArgumentException">Task type is either abstract or not if type <see cref="ITask"/></exception>
        internal TaskBuilder(FlowConfiguration config, string name, Type taskType, LogType logType)
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
            this.TraceType = logType;
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        protected FlowConfiguration Config { get; private set; }

        /// <summary>
        /// Gets the name of the task
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        protected string Name { get; private set; }

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        /// <value>
        /// The type of the task.
        /// </value>
        protected Type TaskType { get; private set; }

        /// <summary>
        /// Gets or sets the type of the trace.
        /// </summary>
        /// <value>
        /// The type of the trace.
        /// </value>
        protected LogType TraceType { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        protected Dictionary<string, object> Parameters { get; private set; }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="name">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <returns>Returns the reference to the task builder <see cref="ITaskBuilder"/></returns>
        public ITaskBuilder AddParameter(string name, object value)
        {
            name.NotNull(nameof(name));
            value.NotNull(nameof(value));

            Parameters[name] = value;
            return this;
        }


        /// <summary>
        /// Add the task to the configuration
        /// </summary>
        public abstract void Add();
    }
}
