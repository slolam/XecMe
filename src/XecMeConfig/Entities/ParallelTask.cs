using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XecMeConfig.Entities
{
    [XmlRoot("parallelTaskRunner")]
    public class ParallelTask : BaseTask
    {
        [XmlElement("minInstances", IsNullable = false)]
        public int MinimumInstances { get; set; }
        [XmlElement("maxInstances", IsNullable = false)]
        public int MaximumInstances { get; set; }
        [XmlElement("idlePollingPeriod", IsNullable = true)]
        public int IdlePollingPeriod { get; set; }
        [XmlElement("dayStartTime", IsNullable = true)]
        public TimeSpan StartTime { get; set; }
        [XmlElement("dayEndTime", IsNullable = true)]
        public TimeSpan EndTime { get; set; }
        [XmlElement("weekdays", IsNullable = true)]
        public string Weekdays { get; set; }
        [XmlElement("timeZone", IsNullable = true)]
        public string TimeZone { get; set; }
    }
}
