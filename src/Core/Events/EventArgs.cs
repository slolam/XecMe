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
