#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ExecutionContext.cs is part of XecMe.
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
using System.Collections.Specialized;
using XecMe.Core.Utils;
using XecMe.Common;
using SimpleInjector;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Instance of this type hold the execution context of the tasks
    /// </summary>
    public class ExecutionContext : Dictionary<string, object>
    {
        /// <summary>
        /// TaskRunner for this task
        /// </summary>
        private TaskRunner _taskRunner;
        /// <summary>
        /// Parameters for this task from configuration
        /// </summary>
        private StringDictionary _parameters;
        /// <summary>
        /// Simple injector container for DI
        /// </summary>
        private static Container _container;
        /// <summary>
        /// Parameters initialized in the config
        /// </summary>
        public StringDictionary Parameters
        {
            get { return _parameters; }
        }

        /// <summary>
        /// Internal Simple Injector container to be used in the Task
        /// </summary>
        internal static Container InternalContainer
        {
            get { return _container; }
            set { _container = value; }
        }

        /// <summary>
        /// Simple Injector container
        /// </summary>
        public Container Container
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
        public ExecutionContext(StringDictionary parameter):this(parameter, null)
        {
        }
        /// <summary>
        /// Constructor to create ExecutionContext from the parameters and the parent TaskRunner instance
        /// </summary>
        /// <param name="parameter">Parameters initialized from the config</param>
        /// <param name="taskRunner">TaskRunner executing the current task</param>
        public ExecutionContext(StringDictionary parameter, TaskRunner taskRunner)
        {
            Guard.ArgumentNotNull(parameter, "parameter");
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
