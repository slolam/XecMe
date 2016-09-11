using System;
using System.Collections.Generic;
using System.Text;

namespace XecMe.Common
{
    /// <summary>
    /// This class is a helper class for creating instances using reflection
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateInstance<T>()
        {
            return CreateInstance<T>(typeof(T));
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string typeName)
        {
            return CreateInstance<T>(typeName, null);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static T CreateInstance<T>(string typeName,params object[] parameters)
        {
            return (T)Activator.CreateInstance(Type.GetType(typeName), parameters);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type type, params object[] parameters)
        {
            return (T)Activator.CreateInstance(type, parameters);
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        public static object CreateInstance(Type type, params object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }
    }
}
