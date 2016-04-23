#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ServiceElement.cs is part of XecMe.
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
using System.Data;

namespace XecMe.Core.Configuration
{
    public class ServiceElement : ConfigurationElement
    {
        #region Const
        private const string NAME = "name";
        private const string POOL = "servicePool";
        private const string SERVICE_TYPE = "serviceType";
        #endregion

        [ConfigurationProperty(NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get
            {
                return (string)base[NAME];
            }
            set
            {
                base[NAME] = value;
            }
        }

        [ConfigurationProperty(SERVICE_TYPE, IsRequired = true, IsKey = false)]
        public string ServiceType
        {
            get
            {
                return (string)base[SERVICE_TYPE];
            }
            set
            {
                base[SERVICE_TYPE] = value;
            }
        }

        [ConfigurationProperty(POOL, IsRequired = false, DefaultValue = "Default"), StringValidator(MinLength = 1)]
        public string Pool
        {
            get
            {
                return (string)base[POOL];
            }
            set
            {
                if (value == null
                    || value.Trim().Length == 0)
                    value = "Default";
                base[POOL] = value;
            }
        }
    }
}
