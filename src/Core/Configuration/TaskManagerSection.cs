using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using XecMe.Configuration;

namespace XecMe.Core.Configuration
{
    /// <summary>
    /// Type to read the task manager oconfiguration used by XecMe
    /// </summary>
    public class TaskManagerSection: ConfigurationSection
    {
        #region Constants
        internal const string TASK_RUNNERS = "taskRunners";
        internal const string TASKS = "tasks";
        internal const string TASK_MANAGER = "taskManager";
        #endregion

        /// <summary>
        /// Gets the TaskManagerSection instance
        /// </summary>
        public static TaskManagerSection ThisSection
        {
            get { 
                return (TaskManagerSection)XecMeSectionGroup.ThisSection.Sections[TASK_MANAGER]; 
            }
        }

        /// <summary>
        /// Gets the task runners collection
        /// </summary>
        [ConfigurationProperty(TASK_RUNNERS, IsRequired = true)]
        [ConfigurationCollection(typeof(TaskRunnerElement))]
        public ReferencedConfigurationElementCollection<TaskRunnerElement> Tasks
        {
            get
            {
                ReferencedConfigurationElementCollection<TaskRunnerElement> tasks = (ReferencedConfigurationElementCollection<TaskRunnerElement>)base[TASK_RUNNERS];
                tasks.Extensions = ExtensionsSection.ThisSection.GetExtensions(TASK_RUNNERS);
                return tasks;
            }
        }

        /// <summary>
        /// Function should not be commented/removed. 
        /// </summary>
        /// <param name="parentElement"></param>
        protected override void Reset(ConfigurationElement parentElement)
        {
            //This function is overrided to suppress the base class behavior
            //base.Reset(parentElement);
        }
    }
}
