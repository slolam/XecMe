using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XecMeConfig.Entities
{
    [XmlRoot("timerTaskRunner")]
    public class TimerTask: BaseTask
    {
        [XmlElement("startDateTime", IsNullable = true)]
        public DateTime StartDateTime { get; set; }
        [XmlElement("endDateTime", IsNullable = true)]
        public DateTime EndDateTime { get; set; }
        [XmlElement("dayStartTime", IsNullable = true)]
        public TimeSpan StartTime { get; set; }
        [XmlElement("dayEndTime", IsNullable = true)]
        public TimeSpan EndTime { get; set; }
        [XmlElement("interval", IsNullable = false)]
        public long Interval { get; set; }
        [XmlElement("recurrence", IsNullable = true)]
        public long Recurrence { get; set; }
        [XmlElement("timeZone", IsNullable=true)]
        public string TimeZone { get; set; }

        [XmlElement("weekdays", IsNullable = true)]
        public string Weekdays { get; set; }
    }
}
