#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file HostingSection.cs is part of XecMe.
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
using System.Collections.ObjectModel;
using XecMe.Configuration;

namespace XecMe.Core.Configuration
{
    public class HostingSection : ConfigurationSection
    {
        #region Constants
        public const string HOSTING_SECTION = "serviceHosting";
        public const string SERVICES_SECTION = "services";
        #endregion

        private List<string> _servicePools;
        public static HostingSection GetHostingSection()
        {
            HostingSection section = (HostingSection)XecMeSectionGroup.ThisSection.Sections[HOSTING_SECTION];

            if (section == null)
                throw new ConfigurationErrorsException(string.Concat(HOSTING_SECTION, " was not configured"));
            return section;
        }

        private static void ValidateSection(HostingSection section)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            for (int i = 0; i < section.Services.Count; i++)
            {
                ServiceElement se = section.Services[i];
                if (string.Compare("Default", se.Pool, true) == 0)
                {
                    continue;
                }
                temp[se.Pool] = se.Pool;
            }
            section._servicePools.AddRange(temp.Keys);
        }


        public ReadOnlyCollection<string> Pools
        {
            get { return this._servicePools.AsReadOnly(); }
        }


        [ConfigurationProperty(SERVICES_SECTION, IsRequired = true)]
        [ConfigurationCollection(typeof(ServiceElement), AddItemName = "service")]
        public ConfigurationElementCollection<ServiceElement> Services
        {
            get
            {
                return (ConfigurationElementCollection<ServiceElement>)base[SERVICES_SECTION];
            }
        }

        public static HostingSection ThisSection
        {
            get { return (HostingSection)XecMeSectionGroup.ThisSection.Sections[HOSTING_SECTION]; }
        }
    }
}
