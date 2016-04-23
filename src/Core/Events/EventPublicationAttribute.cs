#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file EventPublicationAttribute.cs is part of XecMe.
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
    /// Attribute used to annotate the event to be used by the event broker
    /// </summary>
    [AttributeUsage(AttributeTargets.Event, AllowMultiple = true)]
    public sealed class EventPublicationAttribute : Attribute
    {
        /// <summary>
        /// Name of the event topic
        /// </summary>
        private string _topic;

        /// <summary>
        /// Constructor to initialize EventPublicationAttribute instance
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        public EventPublicationAttribute(string topic)
        {
            this._topic = topic;
        }

        /// <summary>
        /// Gets the name of the event topic
        /// </summary>
        public string Topic
        {
            get
            {
                return this._topic;
            }
        }
    }

 

}
