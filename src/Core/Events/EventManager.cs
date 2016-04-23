#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file EventManager.cs is part of XecMe.
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
using XecMe.Common;
using System.Reflection;
using System.Diagnostics;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Events
{
    /// <summary>
    /// Event broker that wire all the events together by ties them through event topic
    /// </summary>
    public static class EventManager
    {
        /// <summary>
        /// Field to hold the event topic name to event topic object
        /// </summary>
        private static Dictionary<string, EventTopic> _eventTopics;

        static EventManager()
        {
            _eventTopics = new Dictionary<string, EventTopic>();
        }

        /// <summary>
        /// Returns the EventTopic object for the event topic is exists else return null
        /// </summary>
        /// <param name="topic">Event topic name</param>
        /// <returns>EventTopic object associated with the event topic</returns>
        public static EventTopic GetEventTopic(string topic)
        {
            if(_eventTopics.ContainsKey(topic))
                return _eventTopics[topic];
            return null;
        }

        /// <summary>
        /// Returns the EventTopic object for the event topic. If does not already exist creates one
        /// </summary>
        /// <param name="topic">Event topic name</param>
        /// <returns>EventTopic object associated with the event topic</returns>
        public static EventTopic CreateOrGetEventTopic(string topic)
        {
            lock (_eventTopics)
            {
                if (!_eventTopics.ContainsKey(topic))
                {
                    _eventTopics.Add(topic, new EventTopic(topic));
                    Log.Information(string.Format("Event topic {0} create", topic));
                }
                return _eventTopics[topic];
            }
        }

        #region Commented
        /*
        public static void RemoveSubscription(string topic, object subscriber)
        {
            Guard.ArgumentNotNullOrEmptyString(topic, "topic");
            Guard.ArgumentNotNull(subscriber, "subscriber");
            EventTopic eventTopic = _eventTopics[topic];
            if (eventTopic != null)
            {
            }
        }

        public static void RemovePublisher(string topic, object publisher)
        {
            Guard.ArgumentNotNullOrEmptyString(topic, "topic");
            Guard.ArgumentNotNull(publisher, "publisher");
            EventTopic eventTopic = _eventTopics[topic];
            if (eventTopic != null)
            {
                eventTopic.ProcessPublisher(publisher,
            }
        }
         * */
        #endregion

        /// <summary>
        /// Processes the object using reflection to wire up event source and sinks
        /// </summary>
        /// <param name="item">Object to be registered</param>
        public static void Register(object item)
        {
            Guard.ArgumentNotNull(item, "item");
            ProcessPublishers(item, true);
            ProcessSubscribers(item, true);
        }

        /// <summary>
        /// Unregisters the object using reflection to remove the event source and sink
        /// </summary>
        /// <param name="item">Object to be unregistered</param>
        public static void Unregister(object item)
        {
            Guard.ArgumentNotNull(item, "item");
            ProcessPublishers(item, false);
            ProcessSubscribers(item, false);
        }

        /// <summary>
        /// Process the object for event sources
        /// </summary>
        /// <param name="item">Object to be processed</param>
        /// <param name="register">Registered if true, else false</param>
        private static void ProcessPublishers(object item, bool register)
        {
            Type itemType = item.GetType();
            lock (_eventTopics)
            {
                foreach (EventInfo info in itemType.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    foreach (EventPublicationAttribute attribute in info.GetCustomAttributes(typeof(EventPublicationAttribute), true))
                    {
                        EventTopic eventTopic = CreateOrGetEventTopic(attribute.Topic);
                        eventTopic.ProcessPublisher(item, info, register);
                    }
                }
            }
        }

        /// <summary>
        /// Process the object for event sinks
        /// </summary>
        /// <param name="item">Object to be processed</param>
        /// <param name="register">Register if true, else false</param>
        private static void ProcessSubscribers(object item, bool register)
        {
            Type itemType = item.GetType();
            lock (_eventTopics)
            {
                foreach (MethodInfo info in itemType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    foreach (EventSubscriptionAttribute attribute in info.GetCustomAttributes(typeof(EventSubscriptionAttribute), true))
                    {
                        EventTopic eventTopic = CreateOrGetEventTopic(attribute.Topic);
                        eventTopic.ProcessSubcriber(item, info, attribute.Thread, register);
                    }
                }
            }
        }

        /// <summary>
        /// Add publishing source for the given event topic and event name. Use this method when EventPublicationAttribute is not used
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        /// <param name="item">Object on which the event is present</param>
        /// <param name="eventName">Name of the event on the object</param>
        public static void AddPublisher(string topic, object item, string eventName)
        {
            ProcessPublisher(topic, item, eventName, true);
        }

        /// <summary>
        /// Removes the publishing source from the given event topic name if exist. Use this method if you want to remove the publishing source
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        /// <param name="item">Object on which the event is present</param>
        /// <param name="eventName">Name of the event on the object</param>
        public static void RemovePublisher(string topic, object item, string eventName)
        {
            ProcessPublisher(topic, item, eventName, false);
        }

        /// <summary>
        /// Registers/unregister the event as publishing source in topic
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        /// <param name="item">Object for the publishing source</param>
        /// <param name="eventName">EVent name on the publishing object</param>
        /// <param name="register">Register if true, else false</param>
        private static void ProcessPublisher(string topic, object item, string eventName, bool register)
        {
            Guard.ArgumentNotNullOrEmptyString(topic, "topic");
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentNotNullOrEmptyString(eventName, "eventName");
            EventTopic eventTopic = CreateOrGetEventTopic(topic);
            EventInfo info = item.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (info == null)
                throw new ArgumentException(string.Format("Event {0} not found in the type {1}", eventName, item.GetType()), "eventName");
            eventTopic.ProcessPublisher(item, info, register);
        }


        /// <summary>
        /// Register named method on the given object for the event topic with the threading option
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        /// <param name="item">Object to be used for event sink</param>
        /// <param name="methodName">Name of the method to be used for the sink</param>
        /// <param name="threadOption">Threading option while consuming the event</param>
        public static void AddSubscriber(string topic, object item, string methodName, ThreadOption threadOption)
        {
            ProcessSubscriber(topic, item, methodName, threadOption, true);
        }

        /// <summary>
        /// Unregister named method on the given object for the event topic
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        /// <param name="item">Object to be unregistered</param>
        /// <param name="methodName">Name of the method</param>
        public static void RemoveSubscriber(string topic, object item, string methodName)
        {
            ProcessSubscriber(topic, item, methodName, ThreadOption.BackgroundParallel, false);
        }

        /// <summary>
        /// Registers/unregister the event sink for event topic
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        /// <param name="item">Object to be processed for event sink</param>
        /// <param name="methodName">Name of the method to be attached</param>
        /// <param name="threadOption">Thread option to be used while processing this event</param>
        /// <param name="register">Register if true, else false</param>
        private static void ProcessSubscriber(string topic, object item, string methodName, ThreadOption threadOption, bool register)
        {
            Guard.ArgumentNotNullOrEmptyString(topic, "topic");
            Guard.ArgumentNotNull(item, "item");
            Guard.ArgumentNotNullOrEmptyString(methodName, "methodName");
            EventTopic eventTopic = CreateOrGetEventTopic(topic);
            MethodInfo info = item.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (info == null)
                throw new ArgumentException(string.Format("Method {0} not found in the type {1}", methodName, item.GetType()), "eventName");
            eventTopic.ProcessSubcriber(item, info, threadOption, register);
        }
    }
}
