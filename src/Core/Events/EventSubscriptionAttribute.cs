#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file EventSubscriptionAttribute.cs is part of XecMe.
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
    /// Attribute used to annotate the method consuming the event to be used by the event broker
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventSubscriptionAttribute : Attribute
    {
        // Fields
        /// <summary>
        /// Threading option to be used for this event sink
        /// </summary>
        private ThreadOption _threadOption;

        private string _topic;

        /// <summary>
        /// Constructor to create the instance with event topic name and publisher thread
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        public EventSubscriptionAttribute(string topic)
            : this(topic, ThreadOption.Publisher)
        {
        }

        /// <summary>
        /// Constructor to create the instance of the event topic name and ThreadOption
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        /// <param name="threadOption">ThreadOption of the event</param>
        public EventSubscriptionAttribute(string topic, ThreadOption threadOption)
        {
            this._topic = topic;
            this._threadOption = threadOption;
        }

        /// <summary>
        /// Gets or sets the ThreadOption
        /// </summary>
        public ThreadOption Thread
        {
            get
            {
                return this._threadOption;
            }
            set
            {
                this._threadOption = value;
            }
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



    #region
    /// <summary>
    /// Thread option for the event
    /// </summary>
    public enum ThreadOption
    {
        /// <summary>
        /// Same thread as the publisher
        /// </summary>
        Publisher,
        /// <summary>
        /// Background serial thread
        /// </summary>
        BackgroundSerial,
        /// <summary>
        /// Background parallel
        /// </summary>
        BackgroundParallel
    }
    #endregion
}
