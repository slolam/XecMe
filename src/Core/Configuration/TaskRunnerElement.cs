using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using XecMe.Core.Tasks;
using XecMe.Configuration;
using System.Collections.Specialized;

namespace XecMe.Core.Configuration
{
    /// <summary>
    /// This type is the base configuration element for all the task runners 
    /// </summary>
    public class TaskRunnerElement: ConfigurationElement
    {
        #region Contants
        private const string TASK_TYPE = "taskType";
        private const string PARAMETERS = "parameters";
        private const string NAME = "name";
        private const string TRACE = "traceFilter";
        #endregion

        /// <summary>
        /// Gets or sets the fully qualified .NET type name that implements the XecMe.CodeTasks.ITask type
        /// </summary>
        [ConfigurationProperty(TASK_TYPE, IsRequired = true)]
        public string TaskType
        {
            get { return (string)base[TASK_TYPE]; }
            set { base[TASK_TYPE] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the task
        /// </summary>
        [ConfigurationProperty(NAME, IsRequired = true, IsKey=true)]
        public string Name
        {
            get { return (string)base[NAME]; }
            set { base[NAME] = value; }
        }

        /// <summary>
        /// Gets or sets the trace configuration for this task runner
        /// </summary>
        [ConfigurationProperty(TRACE, DefaultValue=LogType.All)]
        public LogType TraceFilter
        {
            get { return (LogType)base[TRACE]; }
            set { base[TRACE] = value; }
        }

        /// <summary>
        /// Abstract method to return the TaskRunner instance of this type
        /// </summary>
        /// <returns>Instance of the type TaskRunner</returns>
        public virtual TaskRunner GetRunner() { return null; }

        /// <summary>
        /// Gets the collection of configured parameters for this instance of the task
        /// </summary>
        [ConfigurationProperty(PARAMETERS, IsRequired = true)]
        [ConfigurationCollection(typeof(XecMe.Configuration.KeyValueConfigurationElement), AddItemName = "parameter")]
        public ConfigurationElementCollection<XecMe.Configuration.KeyValueConfigurationElement> Parameters
        {
            get
            {
                return (ConfigurationElementCollection<XecMe.Configuration.KeyValueConfigurationElement>)base[PARAMETERS];
            }
        }

        /// <summary>
        /// Returns the parameters dictionary for this instances
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, object> InternalParameters()
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            foreach (XecMe.Configuration.KeyValueConfigurationElement item in Parameters)
            {
                param.Add(item.Name, item.Value);
            }
            return param;
        }

        /// <summary>
        /// Returns the Type of the task 
        /// </summary>
        /// <returns></returns>
        protected Type GetTaskType()
        {
            return Type.GetType(this.TaskType);
        }

    }
}
