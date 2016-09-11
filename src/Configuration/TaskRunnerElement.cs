using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
//using AmericanExpress.Cuso.Framework.Tasks;
using System.Collections.Specialized;

namespace AmericanExpress.Cuso.Framework.Configuration
{
    public class TaskRunnerElement: ConfigurationElement
    {
        #region Contants
        private const string TASK_TYPE = "taskType";
        private const string PARAMETERS = "parameters";
        private const string NAME = "name";
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

        public virtual TaskRunner GetRunner() { return null; }

        [ConfigurationProperty(PARAMETERS, IsRequired = true)]
        [ConfigurationCollection(typeof(KeyValueConfigurationElement), AddItemName = "parameter")]
        public ConfigurationElementCollection<KeyValueConfigurationElement> Parameters
        {
            get
            {
                return (ConfigurationElementCollection<KeyValueConfigurationElement>)base[PARAMETERS];
            }
        }

        protected StringDictionary InternalParameters()
        {
            StringDictionary param = new StringDictionary();
            foreach (KeyValueConfigurationElement item in Parameters)
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
