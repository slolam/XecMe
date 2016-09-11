using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using XecMe.Configuration;

namespace XecMe.Core.Reporting.Configuration
{
    public class ReportApplication : ConfigurationElement
    {
        // Fields
        private const string DEFINITION = "definitions";
        private const string DESCRIPTION = "description";
        private const string NAME = "name";

        // Properties
        [ConfigurationProperty(DESCRIPTION, IsKey = false, IsRequired = false)]
        public string Description
        {
            get
            {
                return (string)this[DESCRIPTION];
            }
            set
            {
                this[DESCRIPTION] = value;
            }
        }

        [ConfigurationProperty(NAME, IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this[NAME];
            }
            set
            {
                this[NAME] = value;
            }
        }

        [ConfigurationCollection(typeof(ConfigurationElementCollection<ReportDefinition>), AddItemName = "reportDefinition"),
        ConfigurationProperty(DEFINITION, IsKey = false, IsRequired = true)]
        public ConfigurationElementCollection<ReportDefinition> ReportDefinitions
        {
            get
            {
                return (ConfigurationElementCollection<ReportDefinition>)this[DEFINITION];
            }
        }
    }


}
