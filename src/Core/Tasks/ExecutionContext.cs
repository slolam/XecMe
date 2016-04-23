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
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using XecMe.Core.Utils;
using XecMe.Common;

namespace XecMe.Core.Tasks
{
    public class ExecutionContext : Dictionary<string, object>
    {
        private TaskRunner _taskRunner;
        private StringDictionary _parameters;
        /// <summary>
        /// Parameters initialized in the config
        /// </summary>
        public StringDictionary Parameters
        {
            get { return _parameters; }
        }

        public TaskRunner TaskRunner
        {
            get { return _taskRunner; }
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
