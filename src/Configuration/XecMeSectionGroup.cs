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
