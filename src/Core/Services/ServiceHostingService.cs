#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ServiceHostingService.cs is part of XecMe.
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
using System.IO;
using XecMe.Core.Utils;
using XecMe.Core.Configuration;
using XecMe.Core.Tasks;

namespace XecMe.Core.Services
{
    public class ServiceHostingService: IService
    {
        private string _serviceName;
        private static Dictionary<string, AppDomain> _pools;
        private static CrossDomainEventBroker _evtBroker;


        public ServiceHostingService()
        {
            _pools = new Dictionary<string, AppDomain>();
            _evtBroker = new CrossDomainEventBroker();
        }

        internal static void UnloadAllServices()
        {
            lock (_pools)
            {
                foreach (AppDomain app in _pools.Values)
                {
                    AppDomain.Unload(app);
                }
                _pools.Clear();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolName"></param>
        internal static void LoadServicePool(string poolName)
        {
            string key = poolName.ToLower();
            if (_pools.ContainsKey(key))
                throw new InvalidOperationException(string.Concat("Pool ", poolName, " is already created"));

            AppDomainSetup appInfo = new AppDomainSetup();
            AppDomain current = AppDomain.CurrentDomain;

            appInfo.ConfigurationFile = current.SetupInformation.ConfigurationFile; ;
            appInfo.AppDomainInitializer = new AppDomainInitializer(ServiceActivator.LoadAndInitialize);
            appInfo.AppDomainInitializerArguments = new string[] { poolName };
            appInfo.LoaderOptimization = LoaderOptimization.MultiDomain;
            appInfo.ApplicationBase = current.SetupInformation.ApplicationBase;
            appInfo.ApplicationName = current.SetupInformation.ApplicationName;
            appInfo.DisallowCodeDownload = true;

            AppDomain app = AppDomain.CreateDomain(poolName, current.Evidence, appInfo);

            app.UnhandledException += new UnhandledExceptionEventHandler(_evtBroker.UnhandledExceptionSink);
            app.DomainUnload += new EventHandler(_evtBroker.DomainUnloadSink);

            _evtBroker.UnhandledException += new UnhandledExceptionEventHandler(ServicePoolUnhandledException);
            _evtBroker.DomainUnload += new EventHandler(ServicePoolUnload);

            lock (_pools)
            {
                _pools.Add(key, app);
            }
        }

        private static void ServicePoolUnload(object sender, EventArgs e)
        {
            AppDomain app = (AppDomain)sender;
            string key = app.FriendlyName.ToLower();
            if (!_pools.ContainsKey(key))
                LoadServicePool(app.FriendlyName);
        }

        private static void ServicePoolUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            AppDomain app = (AppDomain)sender;
            string key = app.FriendlyName.ToLower();
            if (e.IsTerminating)
            {
                lock (_pools)
                {
                    _pools.Remove(key);
                }
            }
        }



        #region IService Members
        /// <summary>
        /// Gets and sets the Service Name for this Windows Service.
        /// </summary>
        string IService.ServiceName
        {
            get
            {
                return _serviceName;
            }
            set
            {
                _serviceName = value;
            }
        }

        bool IService.CanPauseAndContinue
        {
            get { return false; }
        }

        void IService.OnContinue()
        {
            throw new NotImplementedException();
        }

        void IService.OnPause()
        {
            throw new NotImplementedException();
        }

        void IService.OnStop()
        {
            throw new NotImplementedException();
        }

        void IService.OnShutdown()
        {
            throw new NotImplementedException();
        }

        void IService.OnStart()
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Runs the WCF service as a standalone console process.
        /// </summary>
        internal static void RunAsProcess()
        {
            HostingSection section = HostingSection.ThisSection;
            foreach (string poolName in section.Pools)
            {
                LoadServicePool(poolName);
            }
        }

    }
}
