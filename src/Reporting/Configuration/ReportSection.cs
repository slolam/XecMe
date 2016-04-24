using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using XecMe.Configuration;

namespace XecMe.Core.Reporting.Configuration
{
    public class ReportsSection : ConfigurationSection
    {
        private const string APPLICATIONS = "applications";
        private const string DEFAULT = "default";
        private const string REPORTS_SECTION = "reports";
        internal const string REPORT_RUNNERS = "reportRunners";

        [ConfigurationProperty(DEFAULT, IsKey = false, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this[DEFAULT];
            }
            set
            {
                this[DEFAULT] = value;
            }
        }

        [ConfigurationCollection(typeof(ConfigurationElementCollection<ReportApplication>), AddItemName = "reportApplication"), 
        ConfigurationProperty(APPLICATIONS, IsKey = false, IsRequired = true)]
        public ConfigurationElementCollection<ReportApplication> ReportApplications
        {
            get
            {
                return (ConfigurationElementCollection<ReportApplication>)this[APPLICATIONS];
            }
        }

        public static ReportsSection ThisSection
        {
            get
            {
                return (ReportsSection)XecMeSectionGroup.ThisSection.Sections[REPORTS_SECTION];
            }
        }
    }
}
