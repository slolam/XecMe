#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ConfigurationElementCollection.cs is part of XecMe.
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
using XecMe.Common;

namespace XecMe.Configuration
{
    /// <summary>
    /// This generic class is used to extend the configuration element collection class used for extensibility
    /// </summary>
    /// <typeparam name="T">Derived class of ConfigurationElement that has default constructor</typeparam>
    public class ConfigurationElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
    {
        /// <summary>
        /// Creates and return the instance of the new custom class derived from ConfigurationElement
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return Reflection.CreateInstance<T>();
        }

        /// <summary>
        /// This is used by the custom elements.
        /// </summary>
        /// <param name="reader">XmlReader associated with ConfigurationManager reading the app.config/web.config</param>
        internal void Deserialize(System.Xml.XmlReader reader)
        {
            this.DeserializeElement(reader, false);
        }

        /// <summary>
        /// Returns the value of the key associated with the configuration element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the element associate with the key
        /// </summary>
        /// <param name="key">key to retrieve the element</param>
        /// <returns>Configuration element of the collection</returns>
        public T this[object key]
        {
            get { return (T)base.BaseGet(key); }
        }
    }
}
