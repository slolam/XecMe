using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace XecMe.Configuration
{
    /// <summary>
    /// Custom configuration element that has just 2 properties viz. name and value
    /// </summary>
    public class KeyValueConfigurationElement: ConfigurationElement
    {
        #region Constants
        private const string VALUE = "value";
        private const string NAME = "name";
        #endregion

        /// <summary>
        /// Gets or sets the value of the property associated with the configuration element
        /// </summary>
        [ConfigurationProperty(VALUE, IsRequired = true)]
        public string Value
        {
            get { return (string)base[VALUE]; }
            set { base[VALUE] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the property associated with the configuration element
        /// </summary>
        [ConfigurationProperty(NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base[NAME]; }
            set { base[NAME] = value; }
        }

    }
}
