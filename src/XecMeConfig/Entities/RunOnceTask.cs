using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XecMeConfig.Entities
{
    [XmlRoot("runOnceRunner")]
    public class RunOnceTask: BaseTask
    {
        public int Delay { get; set; }
    }
}
