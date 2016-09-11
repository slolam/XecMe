using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Core.Tasks;
using System.Configuration;
using XecMe.Core.Events;

namespace XecMe.Core.Configuration
{
    /// <summary>
    /// Type to read the Event task configuration
    /// </summary>
    public class EventTaskRunnerElement : TaskRunnerElement
    {
        #region Constants
        private const string EVENT_TOPIC = "eventTopic";
        private const string THREAD_OPTION = "threadOption";
        private const string TIME_OUT = "timeout";
        #endregion

        /// <summary>
        /// Gets or sets the timeout for the event before it stops this task runner instance
        /// </summary>
        [ConfigurationProperty(TIME_OUT, IsRequired = false, DefaultValue=5000)]
        [IntegerValidator(MinValue=1, MaxValue=10000)]
        public int Timeout
        {
            get { return (int)base[TIME_OUT]; }
            set { base[TIME_OUT] = value; }
        }

        /// <summary>
        /// Gets or sets the event topic name for this event task runner instance
        /// </summary>
        [ConfigurationProperty(EVENT_TOPIC, IsRequired = true)]
        public string EventTopic
        {
            get { return (string)base[EVENT_TOPIC]; }
            set { base[EVENT_TOPIC] = value; }
        }

        /// <summary>
        /// Gets or sets the threading option for this event task runner instance
        /// </summary>
        [ConfigurationProperty(THREAD_OPTION, DefaultValue = ThreadOption.BackgroundParallel)]
        public ThreadOption ThreadOption
        {
            get { return (ThreadOption)base[THREAD_OPTION]; }
            set { base[THREAD_OPTION] = value; }
        }

        /// <summary>
        /// Return the Event Task Runner for this task instance
        /// </summary>
        /// <returns>Returns the instance of EventTaskRunner type</returns>
        public override TaskRunner GetRunner()
        {
            return new EventTaskRunner(this.Name, this.GetTaskType(), InternalParameters(), EventTopic, this.ThreadOption, Timeout, TraceFilter);
        }
    }
}
