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
using System.Linq;
using System.Text;
using XecMe.Common;
using System.Reflection;

namespace XecMe.Core.Events
{
    public static class EventManager
    {
        private static Dictionary<string, EventTopic> _eventTopics;

        static EventManager()
        {
            _eventTopics = new Dictionary<string, EventTopic>();
        }

        public static EventTopic GetEventTopic(string topic)
        {
            if(_eventTopics.ContainsKey(topic))
                return _eventTopics[topic];
            return null;
        }

        public static EventTopic CreateOrGetEventTopic(string topic)
        {
            lock (_eventTopics)
            {
                if (!_eventTopics.ContainsKey(topic))
                {
                    _eventTopics.Add(topic, new EventTopic(topic));
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

        public static void Register(object item)
        {
            Guard.ArgumentNotNull(item, "item");
            ProcessPublishers(item, item.GetType(), true);
            ProcessSubscribers(item, item.GetType(), true);
        }

        public static void Unregister(object item)
        {
            Guard.ArgumentNotNull(item, "item");
            ProcessPublishers(item, item.GetType(), false);
            ProcessSubscribers(item, item.GetType(), false);
        }

        private static void ProcessPublishers(object item, Type itemType, bool register)
        {
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

        private static void ProcessSubscribers(object item, Type itemType, bool register)
        {
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

        public static void AddPublisher(string topic, object item, string eventName)
        {
            ProcessPublisher(topic, item, eventName, true);
        }

        public static void RemovePublisher(string topic, object item, string eventName)
        {
            ProcessPublisher(topic, item, eventName, false);
        }

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



        public static void AddSubscriber(string topic, object item, string methodName, ThreadOption threadOption)
        {
            ProcessSubscriber(topic, item, methodName, threadOption, true);
        }

        public static void RemoveSubscriber(string topic, object item, string methodName)
        {
            ProcessSubscriber(topic, item, methodName, ThreadOption.BackgroundParallel, false);
        }

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
