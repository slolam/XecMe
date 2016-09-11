using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using AmericanExpress.Cuso.Framework.Tasks;
//using AmericanExpress.Cuso.Framework.Utils;
using System.Configuration;

namespace AmericanExpress.Cuso.Framework.Configuration
{
    public class ParallelTaskRunnerElement: TaskRunnerElement
    {
        #region Constants
        private const string MIN_INSTANCE = "minInstances";
        private const string MAX_INSTANCE = "maxInstances";
        private const string IDLE_POLLING_PERIOD = "idlePollingPeriod";
        #endregion

        [ConfigurationProperty(IDLE_POLLING_PERIOD, DefaultValue=300000L), LongValidator(MinValue=100)]
        public long IdlePollingPeriod
        {
            get { return (long)base[IDLE_POLLING_PERIOD]; }
            set { base[IDLE_POLLING_PERIOD] = value; }
        }

        [ConfigurationProperty(MIN_INSTANCE, IsRequired = true, DefaultValue=1), IntegerValidator(MinValue=1)]
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
