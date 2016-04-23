using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace XecMeConfig.Entities
{
    [XmlRoot("parameter")]
    public class Parameter
    {
        [XmlElement("name")]
        public string Name { get; set; }
        [XmlElement("value")]
        public string Value { get; set; }
    }
}
