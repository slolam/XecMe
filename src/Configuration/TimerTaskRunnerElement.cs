using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmericanExpress.Cuso.Framework.Tasks;
using System.Configuration;

namespace AmericanExpress.Cuso.Framework.Configuration
{
    public class TimerTaskRunnerElement : TaskRunnerElement
    {
        #region Constants
        private const string INTERVAL = "interval";
        #endregion

        [ConfigurationProperty(INTERVAL, IsRequired = true)]
        public long Interval
        {
            get { return (long)base[INTERVAL]; }
            set { base[INTERVAL] = value; }
        }

        public override TaskRunner GetRunner()
        {
            return new TimerTaskRunner(this.GetTaskType(), InternalParameters(), Interval);
        }
    }
}
