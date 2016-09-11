using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XecMeConfig.Entities
{
    public class BaseTask
    {
        public BaseTask()
        {
            Parameters = new List<Parameter>();
        }
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("taskType")]
        public string TaskType { get; set; }
        [XmlElement("parameters")]
        public List<Parameter> Parameters { get; private set; }
    }
}
