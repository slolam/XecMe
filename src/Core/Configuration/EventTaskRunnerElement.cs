#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file EventTaskRunnerElement.cs is part of XecMe.
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
using XecMe.Core.Tasks;
using System.Configuration;
using XecMe.Core.Events;

namespace XecMe.Core.Configuration
{
    public class EventTaskRunnerElement : TaskRunnerElement
    {
        #region Constants
        private const string EVENT_TOPIC = "eventTopic";
        private const string THREAD_OPTION = "threadOption";
        private const string TIME_OUT = "timeout";
        #endregion

        [ConfigurationProperty(TIME_OUT, IsRequired = false, DefaultValue=5000)]
        [IntegerValidator(MinValue=1, MaxValue=10000)]
        public int Timeout
        {
            get { return (int)base[TIME_OUT]; }
            set { base[TIME_OUT] = value; }
        }

        [ConfigurationProperty(EVENT_TOPIC, IsRequired = true)]
        public string EventTopic
        {
            get { return (string)base[EVENT_TOPIC]; }
            set { base[EVENT_TOPIC] = value; }
        }

        [ConfigurationProperty(THREAD_OPTION, DefaultValue = ThreadOption.BackgroundParallel)]
        public ThreadOption ThreadOption
        {
            get { return (ThreadOption)base[THREAD_OPTION]; }
            set { base[THREAD_OPTION] = value; }
        }

        public override TaskRunner GetRunner()
        {
            return new EventTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), EventTopic, this.ThreadOption, Timeout);
        }
    }
}
