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
