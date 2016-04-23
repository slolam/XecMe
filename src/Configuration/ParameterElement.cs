using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace AmericanExpress.Cuso.Framework.Configuration
{
    public class ParameterElement : ConfigurationElement
    {
        #region Constants
        private const string VALUE = "value";
        private const string NAME = "name";
        #endregion

        [ConfigurationProperty(VALUE, IsRequired = true)]
        public string Value
        {
            get { return (string)base[VALUE]; }
            set { base[VALUE] = value; }
        }

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name
        {
            get { return (string)base[NAME]; }
            set { base[NAME] = value; }
        }

    }
}
