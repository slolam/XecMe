#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TaskRunnerElement.cs is part of XecMe.
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
using XecMe.Core.Tasks;
using XecMe.Configuration;
using System.Collections.Specialized;

namespace XecMe.Core.Configuration
{
    public class TaskRunnerElement: ConfigurationElement
    {
        #region Contants
        private const string TASK_TYPE = "taskType";
        private const string PARAMETERS = "parameters";
        private const string NAME = "name";
        private const string TRACE = "traceFilter";
        #endregion

        [ConfigurationProperty(TASK_TYPE, IsRequired = true)]
        public string TaskType
        {
            get { return (string)base[TASK_TYPE]; }
            set { base[TASK_TYPE] = value; }
        }

        [ConfigurationProperty(NAME, IsRequired = true, IsKey=true)]
        public string Name
        {
            get { return (string)base[NAME]; }
            set { base[NAME] = value; }
        }

        [ConfigurationProperty(TRACE, DefaultValue=TraceType.All)]
        public TraceType TraceFilter
        {
            get { return (TraceType)base[TRACE]; }
            set { base[TRACE] = value; }
        }
        public virtual TaskRunner GetRunner() { return null; }

        [ConfigurationProperty(PARAMETERS, IsRequired = true)]
        [ConfigurationCollection(typeof(XecMe.Configuration.KeyValueConfigurationElement), AddItemName = "parameter")]
        public ConfigurationElementCollection<XecMe.Configuration.KeyValueConfigurationElement> Parameters
        {
            get
            {
                return (ConfigurationElementCollection<XecMe.Configuration.KeyValueConfigurationElement>)base[PARAMETERS];
            }
        }

        protected StringDictionary InternalParameters()
        {
            StringDictionary param = new StringDictionary();
            foreach (XecMe.Configuration.KeyValueConfigurationElement item in Parameters)
            {
                param.Add(item.Name, item.Value);
            }
            return param;
        }

        protected Type GetTaskType()
        {
            return Type.GetType(this.TaskType);
        }

    }
}
