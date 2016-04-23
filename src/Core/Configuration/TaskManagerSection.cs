#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TaskManagerSection.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         01-2013             Shailesh Lolam

#endregion
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
