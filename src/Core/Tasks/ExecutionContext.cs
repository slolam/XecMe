using System.Collections.Generic;
using XecMe.Common;
using XecMe.Common.Injection;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Instance of this type hold the execution context of the tasks
    /// </summary>
    public sealed class ExecutionContext : Dictionary<string, object>
    {
        /// <summary>
        /// TaskRunner for this task
        /// </summary>
        private TaskRunner _taskRunner;
        /// <summary>
        /// Parameters for this task from configuration
        /// </summary>
        private Dictionary<string, object> _parameters;
        /// <summary>
        /// Simple injector container for DI
        /// </summary>
        private static IContainer _container;

        /// <summary>
        /// The event argument
        /// </summary>
        internal object EventArg { get; set; }

        /// <summary>
        /// Events the argument.
        /// </summary>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <returns></returns>
        public TType GetEventArg<TType>()
        {
            return (TType)EventArg;
        }
        /// <summary>
        /// Parameters initialized in the config
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Internal Simple Injector container to be used in the Task
        /// </summary>
        internal static IContainer InternalContainer
        {
            get { return _container; }
            set { _container = value; }
        }

        /// <summary>
        /// Simple Injector container
        /// </summary>
        public IContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// TaskRummer instance running this task
        /// </summary>
        public TaskRunner TaskRunner
        {
            get 
            {
                return _taskRunner; 
            }
            internal set 
            { 
                _taskRunner = value; 
            }
        }
        /// <summary>
        /// Constructor to create ExecutionContext from parameters
        /// </summary>
        /// <param name="parameter">Parameters initialized from the config</param>
        public ExecutionContext(Dictionary<string, object> parameter):this(parameter, null)
        {
        }
        /// <summary>
        /// Constructor to create ExecutionContext from the parameters and the parent TaskRunner instance
        /// </summary>
        /// <param name="parameter">Parameters initialized from the config</param>
        /// <param name="taskRunner">TaskRunner executing the current task</param>
        public ExecutionContext(Dictionary<string, object> parameter, TaskRunner taskRunner)
        {
            parameter.NotNull(nameof(parameter));
            _parameters = parameter;
            _taskRunner = taskRunner;
        }

        /// <summary>
        /// Create the deep copy of this instance 
        /// </summary>
        /// <returns></returns>
        public ExecutionContext Copy()
        {
            ExecutionContext retVal = new ExecutionContext(_parameters, _taskRunner);
            foreach (string key in this.Keys)
            {
                retVal.Add(key, this[key]);
            }
            return retVal;
        }

    }
}
