using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Core.Configuration;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Configuration based task configurations
    /// </summary>
    /// <seealso cref="XecMe.Core.Tasks.ITaskManagerConfig" />
    public class TaskManagerConfig : ITaskManagerConfig
    {
        /// <summary>
        /// The runners
        /// </summary>
        List<TaskRunner> _runners;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskManagerConfig"/> class.
        /// </summary>
        public TaskManagerConfig()
        {
            _runners = new List<TaskRunner>();
            foreach (TaskRunnerElement taskRunner in TaskManagerSection.ThisSection.Tasks)
            {
                _runners.Add(taskRunner.GetRunner());
            }
        }

        #region ITaskManagerConfig Members        

        /// <summary>
        /// Gets the runners.
        /// </summary>
        /// <value>
        /// The runners.
        /// </value>
        IEnumerable<TaskRunner> ITaskManagerConfig.Runners
        {
            get { return _runners.AsReadOnly(); }
        }

        #endregion
    }
}
