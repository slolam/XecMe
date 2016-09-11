using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XecMeConfig.Entities
{
    public enum ThreadOptions { Publisher, BackgroundSerial, BackgroundParallel };

    [XmlRoot("eventTaskRunner")]
    public class EventTask: BaseTask
    {
        public EventTask()
        {
            ThreadOption = ThreadOptions.BackgroundParallel;
        }

        [XmlElement("eventTopic", IsNullable = false)]
        public string EventTopic { get; set; }
        [XmlElement("timeout", IsNullable = true)]
        public int Timeout { get; set; }
        [XmlElement("threadOption", IsNullable = true)]
        public ThreadOptions ThreadOption { get; set; }
    }
}
