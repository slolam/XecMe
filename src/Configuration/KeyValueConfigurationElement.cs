#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file KeyValueConfigurationElement.cs is part of XecMe.
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

namespace XecMe.Configuration
{
    public class KeyValueConfigurationElement: ConfigurationElement
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

        [ConfigurationProperty(NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base[NAME]; }
            set { base[NAME] = value; }
        }

    }
}
