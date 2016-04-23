#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file GenericComparer.cs is part of XecMe
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
using System.Reflection;

namespace XecMe.Common
{
    /// <summary>
    /// This is the Generic Comparer that can be used for sorting of the Generic lists based a property
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericComparer<T> : IComparer<T>
    {
        private string _property;

        /// <summary>
        /// Gets and sets the property name that needs to be used for sorting.
        /// </summary>
        public string Property
        {
            get { return _property; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");
                _property = value;
            }
        }

        /// <summary>
        /// Constructor for creating the object of the this type.
        /// </summary>
        /// <param name="property">Name of the property that needs to be used for comparing</param>
        public GenericComparer(string property)
        {
            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException("property");
            this._property = property;
        }

        #region IComparer<T> Members
        /// <summary>
        /// This method is used for comparing the data
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int IComparer<T>.Compare(T x, T y)
        {
            return this.C(x, y);
        }

        private int C(T x, T y)
        {
            try
            {
                if (x == null && y == null)
                    return 0;
                if (x == null && y != null)
                    return -1;
                if (x != null && y == null)
                    return 1;
                MethodInfo mix = x.GetType().GetMethod("get_" + _property);
                MethodInfo miy = y.GetType().GetMethod("get_" + _property);
                if (mix == null || miy == null)
                    return 0;
                object vx, vy;
                vx = mix.Invoke(x, null);
                vy = mix.Invoke(y, null);
                if (vx == null && vy == null)
                    return 0;
                if (vx == null && vy != null)
                    return -1;
                if (vx != null && vy == null)
                    return 1;

                IComparable cx = vx as IComparable, cy = vy as IComparable;
                if (cx != null && cy != null)
                {
                    return cx.CompareTo(cy);
                }
                return string.Compare(vx.ToString(), vy.ToString());
            }
            catch 
            {
                throw;
            }
        }
        #endregion
    }
}
