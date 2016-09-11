using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XecMeConfig.Entities
{
    [XmlRoot("scheduledTaskRunner")]
    public class ScheduledTask: BaseTask
    {
        [XmlElement("schedule", IsNullable = true)]
        public string Schedule { get; set; }

        [XmlElement("timeZone", IsNullable = true)]
        public string TimeZone { get; set; }

        [XmlElement("startDate", IsNullable = true)]
        public DateTime StartDate { get; set; }

        [XmlElement("taskTime", IsNullable = false)]
        public TimeSpan TaskTime { get; set; }

        [XmlElement("recursion", IsNullable = false)]
        public Recursions Recursion { get; set; }

        [XmlElement("repeat", IsNullable = true)]
        public int Repeat { get; set; }
    }

    public enum Recursions { Daily, Weekly, Monthly }
}
