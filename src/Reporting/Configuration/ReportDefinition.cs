using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using XecMe.Configuration;
using XecMe.Common;

namespace XecMe.Core.Reporting.Configuration
{
    public class ReportDefinition : ConfigurationElement
    {
        // Fields
        private const string ASSEMBLY_NAME = "assemblyName";
        private const string DESCRIPTION = "description";
        private const string NAME = "name";
        private const string REPORT_PATH = "reportLocation";
        private const string RUNNER_TYPE = "runnerType";

        // Properties
        [ConfigurationProperty(ASSEMBLY_NAME, IsKey = false, IsRequired = false)]
        public string AssemblyName
        {
            get
            {
                return (string)this[ASSEMBLY_NAME];
            }
            set
            {
                this[ASSEMBLY_NAME] = value;
            }
        }

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

        [ConfigurationProperty(REPORT_PATH, IsKey = false, IsRequired = true)]
        public string ReportLocation
        {
            get
            {
                return (string)this[REPORT_PATH];
            }
            set
            {
                this[REPORT_PATH] = value;
            }
        }

        [ConfigurationProperty(RUNNER_TYPE, IsKey = false, IsRequired = true)]
        public string RunnerType
        {
            get
            {
                return (string)this[RUNNER_TYPE];
            }
            set
            {
                ExtensionElement extnElement = ExtensionsSection.ThisSection.GetExtensions(ReportsSection.REPORT_RUNNERS)[value];
                Guard.ArgumentNotNull(extnElement, "RunnerType");
                this[RUNNER_TYPE] = value;
            }
        }
    }
}
