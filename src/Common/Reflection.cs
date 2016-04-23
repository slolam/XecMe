#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file Reflection.cs is part of XecMe.
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
    /// This class is a helper class for creating instances using reflection
    /// </summary>
    public static class Reflection
    {

        public static T CreateInstance<T>()
        {
            return CreateInstance<T>(typeof(T));
        }
        public static T CreateInstance<T>(string typeName)
        {
            return CreateInstance<T>(typeName, null);
        }

        public static T CreateInstance<T>(string typeName,params object[] parameters)
        {
            return (T)Activator.CreateInstance(Type.GetType(typeName), parameters);
        }

        public static T CreateInstance<T>(Type type, params object[] parameters)
        {
            return (T)Activator.CreateInstance(type, parameters);
        }
    }
}
