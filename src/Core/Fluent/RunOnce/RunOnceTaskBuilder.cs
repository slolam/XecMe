using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent.RunOnce
{
    /// <summary>
    /// RunOnce task builder
    /// </summary>
    public interface IRunOnceTaskBuilder
    {
        /// <summary>
        /// Delays before the task is triggered.
        /// </summary>
        /// <param name="delay">The delay in milliseconds</param>
        /// <returns>Returns <see cref="ITaskBuilder"/></returns>
        ITaskBuilder Delay(uint delay);
    }

    /// <summary>
    /// Task builder for RunOnce type of task runner
    /// </summary>
    /// <seealso cref="TaskBuilder" />
    /// <seealso cref="IRunOnceTaskBuilder" />
    internal class RunOnceTaskBuilder: TaskBuilder, IRunOnceTaskBuilder
    {
        uint _delay;

        /// <summary>
        /// Initializes a new instance of the <see cref="RunOnceTaskBuilder"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="logType">Type of the log.</param>
        internal RunOnceTaskBuilder(FlowConfiguration config, string name, Type taskType, LogType logType = LogType.All)
            :base(config, name, taskType, logType) { }

        /// <summary>
        /// Delays before the task is triggered.
        /// </summary>
        /// <param name="delay">The delay in milliseconds</param>
        /// <returns>
        /// Returns <see cref="ITaskBuilder" />
        /// </returns>
        ITaskBuilder IRunOnceTaskBuilder.Delay(uint delay)
        {
            _delay = delay;
            return this;
        }

        /// <summary>
        /// Add the task to the configuration
        /// </summary>
        public override void Add()
        {
            Config.InternalRunners.Add(new RunOnceTaskRunner(Name, TaskType, Parameters, _delay, TraceType));
        }
    }
}
