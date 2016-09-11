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
            element.NotNull(nameof(element));

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
