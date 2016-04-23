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
    public class ExecutionContext : Dictionary<string, object>
    {
        private TaskRunner _taskRunner;
        private StringDictionary _parameters;
        private static Container _container;
        /// <summary>
        /// Parameters initialized in the config
        /// </summary>
        public StringDictionary Parameters
        {
            get { return _parameters; }
        }

        internal static Container InternalContainer
        {
            get { return _container; }
            set { _container = value; }
        }

        public Container Container
        {
            get { return _container; }
        }

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
        public ExecutionContext(StringDictionary parameter):this(parameter, null)
        {
        }
        public ExecutionContext(StringDictionary parameter, TaskRunner taskRunner)
        {
            Guard.ArgumentNotNull(parameter, "parameter");
            _parameters = parameter;
            _taskRunner = taskRunner;
        }

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
