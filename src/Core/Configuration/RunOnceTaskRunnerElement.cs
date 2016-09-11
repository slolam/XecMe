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
            return new RunOnceTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), (uint)Delay, TraceFilter);
        }

    }
}
