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
        /// <param name="parameter">parameter to the method</param>
        /// <param name="parameterName">Name of the parameter</param>
        public static void ArgumentNotNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// This method check the parameter passed to it is null of empty
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="parameterName"></param>
        public static void ArgumentNotNullOrEmptyString(string parameter, string parameterName)
        {
            if(string.IsNullOrEmpty(parameter))
                throw new ArgumentNullException(parameterName);
        }
    }
}
