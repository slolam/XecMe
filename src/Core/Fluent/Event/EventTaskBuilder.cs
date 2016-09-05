using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;
using XecMe.Core.Events;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent.Event
{
    /// <summary>
    /// Event task builder interface
    /// </summary>
    public interface IEventTaskBuilder
    {
        /// <summary>
        /// Subscribes the events from topic
        /// </summary>
        /// <param name="topic">The name of the topic</param>
        /// <returns>Returns <see cref="IThreadOption"/></returns>
        IThreadOption SubscribeEventsFrom(string topic);
    }

    /// <summary>
    /// Interface to define thread option
    /// </summary>
    public interface IThreadOption
    {
        /// <summary>
        /// Event be called on thread.
        /// </summary>
        /// <param name="threadOption">The thread option.</param>
        /// <returns>Returns <see cref="IWaitBeforeExit"/></returns>
        IWaitBeforeExit OnThread(ThreadOption threadOption);
    }

    /// <summary>
    /// Interface to define the wait period before exiting the task
    /// </summary>
    public interface IWaitBeforeExit
    {
        /// <summary>
        /// Wait before exiting when task runner is stopped
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Returns <see cref="ITaskBuilder"/></returns>
        ITaskBuilder AndWaitForTimeoutBeforeExit(int timeout);
    }

    /// <summary>
    /// Event task builder
    /// </summary>
    /// <seealso cref="TaskBuilder" />
    /// <seealso cref="IEventTaskBuilder" />
    internal class EventTaskBuilder: TaskBuilder, IEventTaskBuilder, IThreadOption, IWaitBeforeExit
    {
        string _topic;
        ThreadOption _threadOption;
        int _timeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTaskBuilder"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="name">The name.</param>
        /// <param name="taskType">Type of the task.</param>
        /// <param name="logType">Type of the log.</param>
        internal EventTaskBuilder(FlowConfiguration config, string name, Type taskType, LogType logType = LogType.All)
            :base(config, name, taskType, logType)
        {

        }

        /// <summary>
        /// Add the task to the configuration
        /// </summary>
        public override void Add()
        {
            Config.InternalRunners.Add(new EventTaskRunner(Name, TaskType, Parameters, _topic, _threadOption, _timeout, TraceType));
        }

        /// <summary>
        /// Subscribes the events from topic
        /// </summary>
        /// <param name="topic">The name of the topic</param>
        /// <returns>
        /// Returns <see cref="IThreadOption" />
        /// </returns>
        IThreadOption IEventTaskBuilder.SubscribeEventsFrom(string topic)
        {
            topic.NotNullOrWhiteSpace(nameof(topic));
            _topic = topic;
            return this;
        }

        /// <summary>
        /// Event be called on thread.
        /// </summary>
        /// <param name="threadOption">The thread option.</param>
        /// <returns>
        /// Returns <see cref="IWaitBeforeExit" />
        /// </returns>
        IWaitBeforeExit IThreadOption.OnThread(ThreadOption threadOption)
        {
            _threadOption = threadOption;
            return this;
        }

        /// <summary>
        /// Wait before exiting when task runner is stopped
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns>
        /// Returns <see cref="ITaskBuilder" />
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Timeout cannot be negative</exception>
        ITaskBuilder IWaitBeforeExit.AndWaitForTimeoutBeforeExit(int timeout)
        {
            if (timeout < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout cannot be negative");
            _timeout = timeout;
            return this;
        }
    }
}
