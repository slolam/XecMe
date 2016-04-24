#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file EventTopic.cs is part of XecMe.
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
using System.Threading;
using XecMe.Common;
using System.Diagnostics;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Events
{
    /// <summary>
    /// This class does the brokering of the events between event source (publisher) and event sink (subscriber) 
    /// </summary>
    public class EventTopic
    {
        /// <summary>
        /// Name of the event topic
        /// </summary>
        private string _topic;
        //private bool _enabled;
        /// <summary>
        /// List of the subcribers
        /// </summary>
        private List<Subscriber> _subscribers;
        /// <summary>
        /// Delegate type for this event
        /// </summary>
        private Type _delegateType;

        /// <summary>
        /// Constructor to create instance of this type
        /// </summary>
        /// <param name="topic">Name of the event topic</param>
        internal EventTopic(string topic)
        {
            _topic = topic;
            //_enabled = true;
            _subscribers = new List<Subscriber>();
        }

        /// <summary>
        /// Handles the event and cascade the event to subscriber
        /// </summary>
        /// <param name="sender">Object the triggered the event</param>
        /// <param name="args">Arguments sent by the event source</param>
        internal void HandlePublish(object sender, EventArgs args)
        {
            lock (_subscribers)
            {
                Subscriber subscriber;
                for(int index = 0; index  < _subscribers.Count; index++)
                {
                    subscriber = _subscribers[index];
                    try
                    {
                        if (!subscriber.Fire(sender, args))
                        {
                            index--;
                            _subscribers.Remove(subscriber);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error(string.Format("Error while firing the event: {0}", e));
                    }
                }
            }
        }

        /// <summary>
        /// Wires the event source to the event broker
        /// </summary>
        /// <param name="item">Publisher object</param>
        /// <param name="info">Metadata of the event source</param>
        /// <param name="register">Registers if true, else unregisters</param>
        internal void ProcessPublisher(object item, EventInfo info, bool register)
        {
            if (_delegateType == null)
            {
                _delegateType = info.EventHandlerType;
            }
            else
            {
                if (_delegateType != info.EventHandlerType)
                {
                    throw new ArgumentException("The event type associated with this topic earlier does not match current type", "info");
                }
            }
            Delegate handler = Delegate.CreateDelegate(info.EventHandlerType, this, this.GetType().GetMethod("HandlePublish", BindingFlags.Instance | BindingFlags.NonPublic));
            if (register)
            {
                var addMethod = info.GetAddMethod(true);
                addMethod.Invoke(item, new object[] { handler });
                Log.Information(string.Format("Added new subscriber to event topic {0}", _topic));
            }
            else
            {
                var removeMethod = info.GetRemoveMethod(true);
                removeMethod.Invoke(item, new object[] { handler });
                Log.Information(string.Format("Removed new subscriber to event topic {0}", _topic));
            }
        }

        
        /// <summary>
        /// Wires the event subscriber to the event broker
        /// </summary>
        /// <param name="item">Subscriber object</param>
        /// <param name="info">Metadata of the method consuming the event</param>
        /// <param name="threadOption">ThreadOption for processing the event</param>
        /// <param name="register">Registers if true, else unregisters</param>
        internal void ProcessSubcriber(object item, MethodInfo info, ThreadOption threadOption, bool register)
        {
            lock (_subscribers)
            {
                if (register)
                {
                    _subscribers.Add(new Subscriber(this, item, info, threadOption));
                    Log.Information(string.Format("Added new publisher to event topic {0}", _topic));
                }
                else
                {
                    _subscribers.Remove(new Subscriber(this, item, info, threadOption));
                    Log.Information(string.Format("Removed new publisher to event topic {0}", _topic));
                }
            }
        }
        
        #region Subscriber
        /// <summary>
        /// Wrapper on the subcribers
        /// </summary>
        private class Subscriber
        {
            /// <summary>
            /// Weak reference to the event consumer object
            /// </summary>
            private WeakReference _wrSubscriber;
            /// <summary>
            /// Metadata of the method consuming the event
            /// </summary>
            private RuntimeMethodHandle _methodHandle;

            /// <summary>
            /// Runtime type handle of the event consumer object
            /// </summary>
            private RuntimeTypeHandle _typeHandle;
            /// <summary>
            /// ThreadOption for the event to process
            /// </summary>
            private ThreadOption _threadOption;
            /// <summary>
            /// Reference to the parent event broker
            /// </summary>
            private EventTopic _topic;
            /// <summary>
            /// Constructor of the subscriber
            /// </summary>
            /// <param name="topic">Parent EventTopic object</param>
            /// <param name="target">Target subscriber object</param>
            /// <param name="info">Metadate of the method consuming the event</param>
            /// <param name="threadOption">ThreadOption for processing the event</param>
            public Subscriber(EventTopic topic, object target, MethodInfo info, ThreadOption threadOption)
            {
                target.NotNull(nameof(target));
                info.NotNull(nameof(info));
                _wrSubscriber = new WeakReference(target);
                _threadOption = threadOption;
                _topic = topic;
                _typeHandle = target.GetType().TypeHandle;
                _methodHandle = info.MethodHandle;
            }

            /// <summary>
            /// Creates and returns the delegate for the method that consume the event
            /// </summary>
            /// <returns>Returns the Delegate for the method</returns>
            private Delegate CreateDelegate()
            {
                object target = _wrSubscriber.Target;
                if (target != null)
                {
                    return Delegate.CreateDelegate(_topic._delegateType, target, (MethodInfo)MethodBase.GetMethodFromHandle(_methodHandle, _typeHandle));
                }
                return null;
            }

            /// <summary>
            /// Triggers the delegate that first the method on the subcriber
            /// </summary>
            /// <param name="sender">Source object of the event</param>
            /// <param name="args">Argument sent by the source object</param>
            /// <returns>Returns true when the delegate triggered or scheduled on a worker thread. Returns false if the subscriber object is release</returns>
            public bool Fire(object sender, EventArgs args)
            {
                if (_wrSubscriber.Target != null)
                {
                    switch (_threadOption)
                    {
                        case ThreadOption.BackgroundSerial:
                        case ThreadOption.BackgroundParallel:
                            ThreadPool.QueueUserWorkItem(delegate(object state)
                            {
                                object[] eventArgs = (object[])state;
                                Delegate handler = CreateDelegate();
                                if (handler == null)
                                    return;
                                handler.DynamicInvoke(eventArgs);
                            }, new object[] { sender, args });
                            break;
                        case ThreadOption.Publisher:
                            Delegate ptHandler = CreateDelegate();
                            if (ptHandler == null)
                                return false;
                            ptHandler.DynamicInvoke(new object[] { sender, args });
                            break;
                    }
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Compares the passed object with this instance and returns true if same, else false
            /// </summary>
            /// <param name="obj">Subscriber object to be compared</param>
            /// <returns>Returns true if same, else false</returns>
            public override bool Equals(object obj)
            {
                if (obj == null
                    || obj.GetType() != typeof(Subscriber))
                    return false;

                Subscriber other = (Subscriber)obj;
                if (other._wrSubscriber.Target == this._wrSubscriber.Target
                    && other._methodHandle == this._methodHandle)
                    return true;

                return base.Equals(obj);
            }

            /// <summary>
            /// Returns the hash code for this instance
            /// </summary>
            /// <returns>Returns the hash code for this instance</returns>
            public override int GetHashCode()
            {
                return _typeHandle.GetHashCode();
            }
        }
        #endregion
    }
}
