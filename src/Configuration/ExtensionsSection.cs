using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace XecMe.Configuration
{
    /// <summary>
    /// This class reads the configuration extensions and settings
    /// </summary>
    public class ExtensionsSection: ConfigurationSection
    {
        #region Constants
        private const string EXTENSIONS = "extensions";
        private const string SETTINGS = "settings";
        private readonly ConfigurationElementCollection<ExtensionElement> EMPTY_COLL = new ConfigurationElementCollection<ExtensionElement>();
        #endregion

        /// <summary>
        /// Gets the "extensions" section from the app.config/web.config
        /// </summary>
        public static ExtensionsSection ThisSection
        {
            get { return (ExtensionsSection)XecMeSectionGroup.ThisSection.Sections[EXTENSIONS]; }
        }

        /// <summary>
        /// Gets the "settings" from the app.config/web.config
        /// </summary>
        public ConfigurationElementCollection<ExtensionElement> Settings
        {
            get
            {
                ConfigurationElementCollection<ExtensionElement> retVal = (ConfigurationElementCollection<ExtensionElement>)base[SETTINGS];
                
                if (retVal == null)
                    return EMPTY_COLL;

                return retVal;
            }
        }

       
        /// <summary>
        /// Returns the extension collection associated with the property
        /// </summary>
        /// <param name="propName">Property name like taskRunner, reportRunner etc.</param>
        /// <returns></returns>
        public ConfigurationElementCollection<ExtensionElement> GetExtensions(string propName)
        {
            return (ConfigurationElementCollection<ExtensionElement>)base[propName]; 
        }

        
        /// <summary>
        /// Called by the .NET configuration framework to read the app.config/web.config. This method implements the custom reading of the ExtensionSection
        /// </summary>
        /// <param name="elementName">Element name like taskRunner, reportRunner, settings etc.</param>
        /// <param name="reader">XmlReader associated with the confguration reader of the ConfigurationManager</param>
        /// <returns>Return true of it could successfully able to read the configuration, else false</returns>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            this.Properties.Add(new ConfigurationProperty(elementName, typeof(ConfigurationElementCollection<ExtensionElement>),EMPTY_COLL, ConfigurationPropertyOptions.IsKey));

            ((ConfigurationElementCollection<ExtensionElement>)base[elementName]).Deserialize(reader);

            return true;
        }
    }
}
