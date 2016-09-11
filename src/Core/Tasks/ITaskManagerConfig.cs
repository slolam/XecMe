using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Task manager configuration
    /// </summary>
    public interface ITaskManagerConfig
    {
        /// <summary>
        /// Gets the runners.
        /// </summary>
        /// <value>
        /// The runners.
        /// </value>
        IEnumerable<TaskRunner> Runners { get; }
    }
}
