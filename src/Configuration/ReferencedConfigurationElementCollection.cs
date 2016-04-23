#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ReferencedConfigurationElementCollection.cs is part of XecMe.
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
using System.Xml;
using XecMe.Common;

namespace XecMe.Configuration
{
    /// <summary>
    /// This class is used reference based element parsing. Custom define a tag using the extensions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReferencedConfigurationElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
    {
        private ConfigurationElementCollection<ExtensionElement> _extensions;
        public ConfigurationElementCollection<ExtensionElement> Extensions 
        { 
            get { return _extensions;}
            set { _extensions = value; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            ExtensionElement eElement = _extensions[this.AddElementName];
            Type elementType = Type.GetType(eElement.Type);
            return Reflection.CreateInstance<T>(elementType);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            Guard.ArgumentNotNull(element, "element");

            foreach (PropertyInformation information in element.ElementInformation.Properties)
            {
                if (information.IsKey)
                {
                    return information.Value;
                }
            }
            return element.ToString();
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, System.Xml.XmlReader reader)
        {
            if (_extensions == null)
                throw new InvalidOperationException("Extensions are not referenced to this collection.");

            ExtensionElement eElement = _extensions[elementName];
            Type elementType = Type.GetType(eElement.Type);
            if (elementType == null)
                throw new ConfigurationErrorsException(string.Format("Cannot load type \"{0}\"", eElement.Type));
            if (elementType.IsSubclassOf(typeof(T)))
            {
                ///Change the AddElementName to use the default parsing
                this.AddElementName = elementName;
                return base.OnDeserializeUnrecognizedElement(elementName, reader);
            }
            else
            {
                throw new ConfigurationErrorsException(string.Format("Type \"{0}\" is not subclass of \"{1}\"",elementType.ToString(), typeof(T).ToString()));
            }
        }
    }
}
