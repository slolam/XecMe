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

        /// <summary>
        /// Compares the values of both object using reflection
        /// </summary>
        /// <param name="x">Value of object x</param>
        /// <param name="y">Value of object y</param>
        /// <returns></returns>
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
                PropertyInfo mix = x.GetType().GetProperty(_property);
                PropertyInfo miy = y.GetType().GetProperty(_property);
                if (mix == null || miy == null)
                    return 0;
                object vx, vy;
                vx = mix.GetValue(x, null);
                vy = mix.GetValue(y, null);
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
