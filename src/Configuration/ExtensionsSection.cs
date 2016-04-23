#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ExtensionSection.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         01-2013             Shailesh Lolam

#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace XecMe.Configuration
{
    public class ExtensionsSection: ConfigurationSection
    {
        #region Constants
        private const string EXTENSIONS = "extensions";
        private const string SETTINGS = "settings";
        private readonly ConfigurationElementCollection<ExtensionElement> EMPTY_COLL = new ConfigurationElementCollection<ExtensionElement>();
        #endregion

        public static ExtensionsSection ThisSection
        {
            get { return (ExtensionsSection)XecMeSectionGroup.ThisSection.Sections[EXTENSIONS]; }
        }

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

       
        public ConfigurationElementCollection<ExtensionElement> GetExtensions(string propName)
        {
            return (ConfigurationElementCollection<ExtensionElement>)base[propName]; 
        }

        
        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            this.Properties.Add(new ConfigurationProperty(elementName, typeof(ConfigurationElementCollection<ExtensionElement>),EMPTY_COLL, ConfigurationPropertyOptions.IsKey));

            ((ConfigurationElementCollection<ExtensionElement>)base[elementName]).Deserialize(reader);

            return true;
        }
    }
}
