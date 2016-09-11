using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Fluent.Timer;
using XecMe.Core.Fluent.Parallel;
using XecMe.Core.Fluent.RunOnce;
using XecMe.Core.Fluent.Scheduled;
using XecMe.Core.Tasks;
using XecMe.Core.Fluent.Event;

namespace XecMe.Core.Fluent
{
    /// <summary>
    /// Fluent API configuration for building the tasks
    /// </summary>
    /// <seealso cref="ITaskManagerConfig" />
    public class FlowConfiguration : ITaskManagerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlowConfiguration"/> class.
        /// </summary>
        public FlowConfiguration()
        {
            InternalRunners = new List<TaskRunner>();
            
        }

        /// <summary>
        /// Gets the runners.
        /// </summary>
        /// <value>
        /// The runners.
        /// </value>
        public IEnumerable<TaskRunner> Runners
        {
            get
            {
                return InternalRunners;
            }
        }

        /// <summary>
        /// Gets the internal runners.
        /// </summary>
        /// <value>
        /// The internal runners.
        /// </value>
        internal List<TaskRunner> InternalRunners
        {
            get; private set;
        }

        /// <summary>
        /// Returns the Parallel task builder
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IParallelTaskBuilder ParallelTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType: ITask
        {
            return new ParallelTaskBuilder(this, name, typeof(TTaskType), logType);
        }

        /// <summary>
        /// Returns the asynchronous Parallel task builder.
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IParallelTaskBuilder ParallelAsyncTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITaskAsync
        {
            return new ParallelTaskBuilder(this, name, typeof(TTaskType), logType);
        }

        /// <summary>
        /// Returns the Event task builder
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IEventTaskBuilder EventTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITask
        {
            return new EventTaskBuilder(this, name, typeof(TTaskType), logType);
        }

        /// <summary>
        /// Returns the asynchronous Event task builder.
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IEventTaskBuilder EventAsyncTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITaskAsync
        {
            return new EventTaskBuilder(this, name, typeof(TTaskType), logType);
        }

        /// <summary>
        /// Returns the RunOnce task builder.
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IRunOnceTaskBuilder RunOnce<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITask
        {
            return new RunOnceTaskBuilder(this, name, typeof(TTaskType), logType);
        }


        /// <summary>
        /// Returns the asynchronous Run once task builder.
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IRunOnceTaskBuilder RunOnceAsync<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITaskAsync
        {
            return new RunOnceTaskBuilder(this, name, typeof(TTaskType), logType);
        }

        /// <summary>
        /// Returns the Scheduled task builder
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IScheduledTaskBuilder ScheduledTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITask
        {
            return new ScheduledTaskBuilder(this, name, typeof(TTaskType), logType);
        }

        /// <summary>
        /// Returns asynchronous Scheduled task builder.
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public IScheduledTaskBuilder ScheduledAsyncTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITaskAsync
        {
            return new ScheduledTaskBuilder(this, name, typeof(TTaskType), logType);
        }

        /// <summary>
        /// Returns the Timer task builder.
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public ITimerTaskBuilder TimerTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITask
        {
            return new TimerTaskBuilder(this, name, typeof(TTaskType), logType);
        }


        /// <summary>
        /// Returns the asynchronous Timers task builder.
        /// </summary>
        /// <typeparam name="TTaskType">The type of the task type.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="logType">Type of the log.</param>
        /// <returns></returns>
        public ITimerTaskBuilder TimerAsyncTask<TTaskType>(string name, LogType logType = LogType.None) where TTaskType : ITaskAsync
        {
            return new TimerTaskBuilder(this, name, typeof(TTaskType), logType);
        }
    }
}
