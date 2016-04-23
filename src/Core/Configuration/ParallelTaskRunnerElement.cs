#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ParallelTaskRunnerElement.cs is part of XecMe.
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
using XecMe.Core.Tasks;
using XecMe.Core.Utils;
using System.Configuration;

namespace XecMe.Core.Configuration
{
    public class ParallelTaskRunnerElement: TaskRunnerElement
    {
        #region Constants
        private const string MIN_INSTANCE = "minInstances";
        private const string MAX_INSTANCE = "maxInstances";
        private const string IDLE_POLLING_PERIOD = "idlePollingPeriod";
        private const string SINGLETON = "singleton";
        #endregion

        [ConfigurationProperty(IDLE_POLLING_PERIOD, DefaultValue=300000L), LongValidator(MinValue=100)]
        public long IdlePollingPeriod
        {
            get { return (long)base[IDLE_POLLING_PERIOD]; }
            set { base[IDLE_POLLING_PERIOD] = value; }
        }

        [ConfigurationProperty(SINGLETON, DefaultValue = false)]
        public bool Singleton
        {
            get { return (bool)base[SINGLETON]; }
            set { base[SINGLETON] = value; }
        }

        [ConfigurationProperty(MIN_INSTANCE, IsRequired = true, DefaultValue = 1), IntegerValidator(MinValue = 1)]
        public int MinInstances
        {
            get { return (int)base[MIN_INSTANCE]; }
            set { base[MIN_INSTANCE] = value; }
        }

        [ConfigurationProperty(MAX_INSTANCE, IsRequired = true, DefaultValue=1), IntegerValidator(MinValue=1)]
        public int MaxInstances
        {
            get { return (int)base[MAX_INSTANCE]; }
            set { base[MAX_INSTANCE] = value; }
        }

        public override TaskRunner GetRunner()
        {
            return new ParallelTaskRunner(this.GetTaskType(), InternalParameters(), MinInstances, MaxInstances, IdlePollingPeriod);
        }

    }
}
