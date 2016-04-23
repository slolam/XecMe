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
    [Serializable]
    public class EventArgs<T>: EventArgs
    {
        private T _value;
        public EventArgs(T value)
        {
            _value = value;
        }

        public T Value
        {
            get { return _value; }
        }
    }

    [Serializable]
    public delegate void EventHandler<T>(object sender, EventArgs<T> arg);
}
