using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace AmericanExpress.Cuso.Framework.Configuration
{
    public class TaskManagerSection: ConfigurationSection
    {
        #region Constants
        internal const string TASK_RUNNERS = "taskRunners";
        #endregion

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                return base.Properties;
            }
        }


        [ConfigurationProperty(TASK_RUNNERS, IsRequired = true)]
        [ConfigurationCollection(typeof(TaskRunnerElement))]
        public ReferencedConfigurationElementCollection<TaskRunnerElement> TaskRunners
        {
            get
            {
                ReferencedConfigurationElementCollection<TaskRunnerElement> taskRunners = (ReferencedConfigurationElementCollection<TaskRunnerElement>)base[TASK_RUNNERS];
                taskRunners.Extensions = CusoFrameworkSectionGroup.GetCusoFrameworkSectionGroup().Extensions.TaskRunners;
                return taskRunners;
            }
        }
    }
}
