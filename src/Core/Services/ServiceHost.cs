#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ServiceHost.cs is part of XecMe.
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using XecMe.Common;

namespace XecMe.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ServiceHost : ServiceBase
    {
        private IService _service;
        public ServiceHost(string serviceName, IService service)
        {
            Guard.ArgumentNotNullOrEmptyString(serviceName, "serviceName");
            Guard.ArgumentNotNull(service, "service");
            this.AutoLog = true;
            this.ServiceName = serviceName;
            this._service = service;
            this.CanPauseAndContinue = _service.CanPauseAndContinue;
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _service.OnStart();
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            _service.OnStop();
            base.OnStop();
        }

        protected override void OnContinue()
        {
            _service.OnContinue();
            base.OnContinue();
        }

        protected override void OnPause()
        {
            _service.OnPause();
            base.OnPause();
        }

        protected override void OnShutdown()
        {
            _service.OnShutdown();
            base.OnShutdown();
        }
    }
}
