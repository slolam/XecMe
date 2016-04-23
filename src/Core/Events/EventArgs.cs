#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file EventArgs.cs is part of XecMe.
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

namespace XecMe.Core.Events
{
    /// <summary>
    /// Generic event type used by Events
    /// </summary>
    /// <typeparam name="T">Type of the object used for the event</typeparam>
    [Serializable]
    public class EventArgs<T>: EventArgs
    {
        /// <summary>
        /// Value of the event argument
        /// </summary>
        private T _value;
        /// <summary>
        /// Constructor for the event arguments
        /// </summary>
        /// <param name="value"></param>
        public EventArgs(T value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value of the event argument
        /// </summary>
        public T Value
        {
            get { return _value; }
        }
    }

    /// <summary>
    /// Delegate used for the event
    /// </summary>
    /// <typeparam name="T">Type of the object used in the event</typeparam>
    /// <param name="sender">Object raising the event</param>
    /// <param name="arg">Object being passed with the event</param>
    [Serializable]
    public delegate void EventHandler<T>(object sender, EventArgs<T> arg);
}
