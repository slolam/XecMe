#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file Guard.cs is part of XecMe.
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

namespace XecMe.Common
{
    /// <summary>
    /// This static class is used for performing the validation on the parameters passed to the methods
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Checks whether the parameter is null
        /// </summary>
        /// <param name="parameter">Value of the parameter</param>
        /// <param name="parameterName">Name of the parameter</param>
        [Obsolete("Please use NotNull() instead")]
        public static void ArgumentNotNull(this object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// This method check the parameter passed to it is null or empty
        /// </summary>
        /// <param name="parameter">Value of the string parameter</param>
        /// <param name="parameterName">Name of the string parameter</param>
        [Obsolete("Please use NotNullOrEmpty() instead")]
        public static void ArgumentNotNullOrEmptyString(this string parameter, string parameterName)
        {
            if(string.IsNullOrEmpty(parameter))
                throw new ArgumentNullException(parameterName);
        }


        /// <summary>
        /// Checks whether the parameter is null
        /// </summary>
        /// <param name="parameter">Value of the parameter</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="message">Error message</param>
        public static void NotNull(this object parameter, string parameterName, string message = null)
        {
            if (parameter == null)
            {
                if (string.IsNullOrWhiteSpace(message))
                    throw new ArgumentNullException(parameterName);
                else
                    throw new ArgumentNullException(parameterName, message);
            }
        }

        /// <summary>
        /// This method check the parameter passed to it is null or empty
        /// </summary>
        /// <param name="parameter">Value of the string parameter</param>
        /// <param name="parameterName">Name of the string parameter</param>
        /// <param name="message">Error message</param>
        public static void NotNullOrEmpty(this string parameter, string parameterName, string message = null)
        {
            if (string.IsNullOrEmpty(parameter))
            {
                if (string.IsNullOrWhiteSpace(message))
                    throw new ArgumentNullException(parameterName);
                else
                    throw new ArgumentNullException(parameterName, message);
            }
        }

        /// <summary>
        /// This method check the parameter passed to it is null, empty or white space
        /// </summary>
        /// <param name="parameter">Value of the string parameter</param>
        /// <param name="parameterName">Name of the string parameter</param>
        /// <param name="message">Error message</param>
        public static void NotNullOrWhiteSpace(this string parameter, string parameterName, string message = null)
        {
            if (string.IsNullOrWhiteSpace(parameter))
            {
                if (string.IsNullOrWhiteSpace(message))
                    throw new ArgumentNullException(parameterName);
                else
                    throw new ArgumentNullException(parameterName, message);
            }
        }
    }
}
