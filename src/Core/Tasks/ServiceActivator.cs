#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ServiceActivator.cs is part of XecMe.
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
using System.ServiceModel;
using XecMe.Configuration;
using XecMe.Core.Configuration;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// This is the Service Activator and is responsible to start the services 
    /// pertaining to current app domain. The service hosting information is read 
    /// from the custom configuration serviceHosting
    /// </summary>
    internal class ServiceActivator : IDisposable
    {
        private string _pool;
        private Dictionary<string, ServiceHost> _services;
        private bool _isOpened = false;
        private static ServiceActivator _instance;
        private ServiceActivator(string pool)
        {
            if (pool == null
                || pool.Trim().Length == 0)
            {
                throw new ArgumentException("pool");
            }
            if (AppDomain.CurrentDomain.FriendlyName != pool)
            {
                throw new InvalidOperationException("Pool is not hosted correctly");
            }
            _pool = pool;

            CreateServiceHost();
        }

        public static void LoadAndInitialize(string[] args)
        {
            if (_instance != null)
                throw new InvalidOperationException("Already loaded in this Application Domain");
            _instance = new ServiceActivator(args[0]);
            _instance.Open();
        }

        public void Open()
        {
            if (_isOpened)
                return;
            if (_services == null)
                CreateServiceHost();
            try
            {
                foreach (string s in _services.Keys)
                {
                    ServiceHost service = _services[s];
                    service.Open();
                }
                _isOpened = true;
            }
            catch
            {
                UnloadServiceHost();
                throw;
            }
        }

        public void Close()
        {
            if (!_isOpened || _services == null)
                return;
            try
            {
                foreach (string s in _services.Keys)
                {
                    ServiceHost service = _services[s];
                    if (service.State == CommunicationState.Opened)
                        service.Close();
                }
                _isOpened = false;
            }
            catch
            {
                UnloadServiceHost();
                throw;
            }
        }

        private void CreateServiceHost()
        {
            if (_services != null)
                UnloadServiceHost();

            _services = new Dictionary<string, ServiceHost>();
            ConfigurationElementCollection<ServiceElement> services = HostingSection.ThisSection.Services;
            foreach (ServiceElement service in services)
            {
                if (string.Compare(service.Pool, _pool, true) == 0)
                {
                    Type serviceType = Type.GetType(service.ServiceType);
                    if (serviceType == null)
                        throw new TypeLoadException(string.Format("Error loading {0} type", service.Name));
                    ServiceHost host = new ServiceHost(serviceType);

                    host.Faulted += new EventHandler(ServiceFaulted);
                    host.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>(ServiceUnknownMessageReceived);

                    _services.Add(service.Name, host);
                }
            }
        }

        void ServiceUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        void ServiceFaulted(object sender, EventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        private void UnloadServiceHost()
        {
            try
            {
                foreach (string service in _services.Keys)
                {
                    try
                    {
                        using (ServiceHost s = _services[service])
                        {
                            s.Faulted -= new EventHandler(ServiceFaulted);
                            s.UnknownMessageReceived -= new EventHandler<UnknownMessageReceivedEventArgs>(ServiceUnknownMessageReceived);
                        }
                    }
                    catch { }
                }
            }
            finally
            {
                _services = null;
            }
        }


        #region IDisposable Members

        void IDisposable.Dispose()
        {
            UnloadServiceHost();
        }

        #endregion
    }
}
