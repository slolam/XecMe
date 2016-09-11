using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace XecMe.Configuration
{
    /// <summary>
    /// This class is used to create extension elements
    /// </summary>
    public class ExtensionElement: ConfigurationElement
    {
        #region Constants
        private const string TYPE = "type";
        private const string NAME = "name";
        #endregion

        /// <summary>
        /// Gets or sets the type string of the configuration extension element
        /// </summary>
        [ConfigurationProperty(TYPE, IsRequired=true)]
        public string Type
        {
            get { return (string)base[TYPE];}
            set { base[TYPE] = value; }
        }

        /// <summary>
        /// Gets or sets the name associated with this configuration extension element
        /// </summary>
        [ConfigurationProperty(NAME, IsRequired = true, IsKey=true)]
        public string Name
        {
            get { return (string)base[NAME]; }
            set { base[NAME] = value; }
        }

    }
}
