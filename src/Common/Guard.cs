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
