#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file XecMeSectionGroup.cs is part of XecMe.
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
    /// <summary>
    /// Class to read the XecMe section group
    /// </summary>
    public class XecMeSectionGroup: ConfigurationSectionGroup
    {
        #region Constants
        private const string SECTION_GROUP = "xecMe.Core";
        #endregion
        /// <summary>
        /// Reference to the XecMe configuration section group 
        /// </summary>
        private static XecMeSectionGroup _section;

        /// <summary>
        /// Type initializer for the XecMe section group
        /// </summary>
        static XecMeSectionGroup()
        {
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _section = (XecMeSectionGroup)config.GetSectionGroup(SECTION_GROUP);
        }
        /// <summary>
        /// Gets the XecMe section group
        /// </summary>
        public static XecMeSectionGroup ThisSection
        {
            get
            {
                return _section;
            }
        }
    }
}
