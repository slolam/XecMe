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
using System.Text;
using XecMe.Core.Tasks;
using XecMe.Core.Utils;
using System.Configuration;

namespace XecMe.Core.Configuration
{
    /// <summary>
    /// Type to read the Run Once Task runner configuration
    /// </summary>
    public class RunOnceTaskRunnerElement : TaskRunnerElement
    {
        #region Constants
        private const string DELAY = "delay";
        #endregion

        /// <summary>
        /// Gets or sets the delay
        /// </summary>
        [ConfigurationProperty(DELAY, DefaultValue=1000), IntegerValidator(MinValue=0)]
        public int Delay
        {
            get { return (int)base[DELAY]; }
            set { base[DELAY] = value; }
        }

        /// <summary>
        /// REturns the instance of RunOnceTaskRunner type
        /// </summary>
        /// <returns>Returns the RunOnceTaskRunner instance</returns>
        public override TaskRunner GetRunner()
        {
            return new RunOnceTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), Delay, TraceFilter);
        }

    }
}
