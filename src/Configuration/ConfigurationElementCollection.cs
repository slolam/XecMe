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
using System.Linq;
using System.Text;
using System.Configuration;
using XecMe.Common;

namespace XecMe.Configuration
{
    public class ConfigurationElementCollection<T> : ConfigurationElementCollection where T : ConfigurationElement, new()
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return Reflection.CreateInstance<T>();
        }

        /// <summary>
        /// This is used by the custom elements.
        /// </summary>
        /// <param name="reader"></param>
        internal void Deserialize(System.Xml.XmlReader reader)
        {
            this.DeserializeElement(reader, false);
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

        public T this[object key]
        {
            get { return (T)base.BaseGet(key); }
        }
    }
}
