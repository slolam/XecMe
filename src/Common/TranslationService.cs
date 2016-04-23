#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file Translation.cs is part of XecMe.
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
using System.Reflection;
using System.Globalization;
using System.Data;
using System.Collections.Specialized;

namespace XecMe.Common.Data
{
    class Method
    {
        public MethodInfo MI;
        public Type Type;
    }
    public static class TranslationService
    {
        #region Using Data Reader
        public static T PopulateObject<T>(ValueReader reader, Dictionary<string, string> columnPropertiesMap)
        {
            T retVal = default(T);
            if (reader.Read())
            {
                retVal = Reflection.CreateInstance<T>();
                Type retType = typeof(T);
                foreach (string columnName in columnPropertiesMap.Keys)
                {
                    string property = columnPropertiesMap[columnName];
                    MethodInfo mi = retType.GetMethod("set_" + property);
                    if (mi == null)
                    {
                        throw new NotImplementedException(string.Format("Property: {0} is not implemented on Type: {1}", property, retType));
                    }
                    Type pt = mi.GetParameters()[0].ParameterType;
                    object val = reader.GetValue(columnName);
                    if (val == null || string.IsNullOrEmpty(val.ToString()))
                    {
                        if (pt.IsValueType)
                            val = Reflection.CreateInstance<object>(pt);
                        else
                            val = null;
                    }
                    if (val != null && pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        val = Convert.ChangeType(val, pt.GetGenericArguments()[0]);
                    }
                    mi.Invoke(retVal, BindingFlags.SetProperty, null, new object[] { val }, CultureInfo.CurrentCulture);
                }
            }
            return retVal;
        }

        public static List<T> PopulateObjectList<T>(ValueReader reader, Dictionary<string, string> columnPropertiesMap)
        {
            List<T> retVal = new List<T>();
            PopulateObjectList<T>(reader, columnPropertiesMap, retVal);
            return retVal;
        }

        public static void PopulateObjectList<T>(ValueReader reader, Dictionary<string, string> columnPropertiesMap, List<T> list)
        {
            List<T> retVal = list;
            LinkedList<T> linkList = new LinkedList<T>();
            KeyValuePair<string, Method>[] miArray = new KeyValuePair<string, Method>[(columnPropertiesMap.Count - 1) + 1];
            Type entType = typeof(T);
            int mapIndex = 0;
            foreach (string columnName  in columnPropertiesMap.Keys)
            {
                string property = columnPropertiesMap[columnName];
                MethodInfo mi = entType.GetMethod("set_" + property);
                if (mi == null)
                {
                    throw new NotImplementedException(string.Format("Property: {0} is not implemented on Type: {1}", property, entType));
                }
                miArray[mapIndex++] = new KeyValuePair<string, Method>(columnName, new Method() { MI = mi, Type = mi.GetParameters()[0].ParameterType});
            }
            while (reader.Read())
            {
                T entity = Reflection.CreateInstance<T>();
                for (int propIndex = 0; propIndex < miArray.Length; propIndex++)
                {
                    Method mi = miArray[propIndex].Value;
                    object val = reader.GetValue(miArray[propIndex].Key);
                    Type pt = mi.Type;
                    if (val == null || string.IsNullOrEmpty(val.ToString()))
                    {
                        if (mi.Type.IsValueType)
                            val = Reflection.CreateInstance<object>(mi.Type);
                        else
                            val = null;
                    }
                    if (val != null && mi.Type.IsGenericType && mi.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        val = Convert.ChangeType(val, mi.Type.GetGenericArguments()[0]);
                    }
                    mi.MI.Invoke(entity, BindingFlags.SetProperty, null, new object[] { val }, CultureInfo.CurrentCulture);
                }
                linkList.AddLast(entity);
            }
            retVal.Capacity = (retVal.Count + linkList.Count) + 10;
            foreach (T entity in linkList)
            {
                retVal.Add(entity);
            }
            linkList.Clear();
            if (retVal.Count > 1000)
            {
                GC.Collect(2, GCCollectionMode.Forced);
            }
        }

        public static List<T> PopulateObjectList<T>(ValueReader reader, string columnName)
        {
            List<T> retVal = new List<T>();
            while (reader.Read())
            {
                retVal.Add((T)reader.GetValue(columnName));
            }
            return retVal;
        }
        #endregion

        #region Using DataView
        public static T PopulateObject<T>(DataView dataView, Dictionary<string, string> columnPropertiesMap)
        {
            T retVal = default(T);
            if (dataView.Count > 0)
            {
                retVal = Reflection.CreateInstance<T>();
                Type retType = typeof(T);
                DataRowView drv = dataView[0];
                foreach (string columnName in columnPropertiesMap.Keys)
                {
                    string property = columnPropertiesMap[columnName];
                    MethodInfo mi = retType.GetMethod("set_" + property);
                    if (mi == null)
                    {
                        throw new NotImplementedException(string.Format("Property: {0} is not implemented on Type: {1}", property, retType));
                    }
                    Type pt = mi.GetParameters()[0].ParameterType;
                    object val = drv[columnName];
                    if (val == null || string.IsNullOrEmpty(val.ToString()))
                    {
                        if (pt.IsValueType)
                            val = Reflection.CreateInstance<object>(pt);
                        else
                            val = null;
                    }
                    if (val != null && pt.IsGenericType && pt.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        val = Convert.ChangeType(val, pt.GetGenericArguments()[0]);
                    }
                    mi.Invoke(retVal, BindingFlags.SetProperty, null, new object[] { val }, CultureInfo.CurrentCulture);
                }
            }
            return retVal;
        }

        public static List<T> PopulateObjectList<T>(DataView dataView, Dictionary<string, string> columnPropertiesMap)
        {
            List<T> retVal = new List<T>();
            PopulateObjectList<T>(dataView, columnPropertiesMap, retVal);
            return retVal;
        }

        public static void PopulateObjectList<T>(DataView dataView, Dictionary<string, string> columnPropertiesMap, List<T> list)
        {
            List<T> retVal = list;
            LinkedList<T> linkList = new LinkedList<T>();
            KeyValuePair<string, Method>[] miArray = new KeyValuePair<string, Method>[(columnPropertiesMap.Count - 1) + 1];
            Type entType = typeof(T);
            int mapIndex = 0;
            foreach (string columnName in columnPropertiesMap.Keys)
            {
                string property = columnPropertiesMap[columnName];
                MethodInfo mi = entType.GetMethod("set_" + property);
                if (mi == null)
                {
                    throw new NotImplementedException(string.Format("Property: {0} is not implemented on Type: {1}", property, entType));
                }
                miArray[mapIndex++] = new KeyValuePair<string, Method>(columnName, new Method() { MI = mi, Type = mi.GetParameters()[0].ParameterType });
            }
            DataRowView drv;
            for (int rowIndex = 0; rowIndex < dataView.Count; rowIndex++)
            {
                drv = dataView[rowIndex];
                T entity = Reflection.CreateInstance<T>();
                for (int propIndex = 0; propIndex < miArray.Length; propIndex++)
                {
                    Method mi = miArray[propIndex].Value;
                    object val = drv[miArray[propIndex].Key];
                    Type pt = mi.Type;
                    if (val == null || string.IsNullOrEmpty(val.ToString()))
                    {
                        if (mi.Type.IsValueType)
                            val = Reflection.CreateInstance<object>(mi.Type);
                        else
                            val = null;
                    }
                    if (val != null && mi.Type.IsGenericType && mi.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        val = Convert.ChangeType(val, mi.Type.GetGenericArguments()[0]);
                    }
                    mi.MI.Invoke(entity, BindingFlags.SetProperty, null, new object[] { val }, CultureInfo.CurrentCulture);
                }
                linkList.AddLast(entity);
            }
            retVal.Capacity = (retVal.Count + linkList.Count) + 10;
            foreach (T entity in linkList)
            {
                retVal.Add(entity);
            }
            linkList.Clear();
            if (retVal.Count > 1000)
            {
                GC.Collect(2, GCCollectionMode.Forced);
            }
        }

        public static List<T> PopulateObjectList<T>(DataView dataView, string columnName)
        {
            List<T> retVal = new List<T>();
            for(int index = 0; index < dataView.Count; index++)
            {
                DataRowView drv = dataView[index];
                T val = default(T);
                if (!drv.Row.IsNull(columnName))
                {
                    val = (T)drv[columnName];
                }
                retVal.Add(val);
            }
            return retVal;
        }
        #endregion
    }
}
