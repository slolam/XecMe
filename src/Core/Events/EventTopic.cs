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
using XecMe.Core.Diagnostics;

namespace XecMe.Core.Events
{
    public class EventTopic
    {
        private string _topic;
        //private bool _enabled;
        private List<Subcriber> _subscribers;
        private Type _delegateType;

        internal EventTopic(string topic)
        {
            _topic = topic;
            //_enabled = true;
            _subscribers = new List<Subcriber>();
        }


        internal void HandlePublish(object sender, EventArgs args)
        {
            lock (_subscribers)
            {
                Subcriber subscriber;
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

        

        internal void ProcessSubcriber(object item, MethodInfo info, ThreadOption threadOption, bool register)
        {
            lock (_subscribers)
            {
                if (register)
                {
                    _subscribers.Add(new Subcriber(this, item, info, threadOption));
                    Log.Information(string.Format("Added new publisher to event topic {0}", _topic));
                }
                else
                {
                    _subscribers.Remove(new Subcriber(this, item, info, threadOption));
                    Log.Information(string.Format("Removed new publisher to event topic {0}", _topic));
                }
            }
        }
        
        #region Subscriber
        private class Subcriber
        {
            private WeakReference _wrSubscriber;
            private RuntimeMethodHandle _methodHandle;
            private RuntimeTypeHandle _typeHandle;
            private ThreadOption _threadOption;
            private EventTopic _topic;
            public Subcriber(EventTopic topic, object target, MethodInfo info, ThreadOption threadOption)
            {
                Guard.ArgumentNotNull(target, "target");
                Guard.ArgumentNotNull(info, "info");
                _wrSubscriber = new WeakReference(target);
                _threadOption = threadOption;
                _topic = topic;
                _typeHandle = target.GetType().TypeHandle;
                _methodHandle = info.MethodHandle;
            }

            private Delegate CreateDelegate()
            {
                object target = _wrSubscriber.Target;
                if (target != null)
                {
                    return Delegate.CreateDelegate(_topic._delegateType, target, (MethodInfo)MethodBase.GetMethodFromHandle(_methodHandle, _typeHandle));
                }
                return null;
            }

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

            public override bool Equals(object obj)
            {
                if (obj == null
                    || obj.GetType() != typeof(Subcriber))
                    return false;

                Subcriber other = (Subcriber)obj;
                if (other._wrSubscriber.Target == this._wrSubscriber.Target
                    && other._methodHandle == this._methodHandle)
                    return true;

                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return _typeHandle.GetHashCode();
            }
        }
        #endregion
    }
}
