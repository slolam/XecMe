#if !NETSTANDARD2_0

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
using XecMe.Common.Diagnostics;

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
            serviceName.NotNullOrEmpty(nameof(serviceName));
            service.NotNull(nameof(service));
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
            Log.Information(string.Format("Service \"{0}\" has started", ServiceName));
        }

        protected override void OnStop()
        {
            _service.OnStop();
            base.OnStop();
            Log.Information(string.Format("Service \"{0}\" has stopped", ServiceName));
        }

        protected override void OnContinue()
        {
            _service.OnContinue();
            base.OnContinue();
            Log.Information(string.Format("Service \"{0}\" has resumed", ServiceName));
        }

        protected override void OnPause()
        {
            _service.OnPause();
            base.OnPause();
            Log.Information(string.Format("Service \"{0}\" has paused", ServiceName));
        }

        protected override void OnShutdown()
        {
            _service.OnShutdown();
            base.OnShutdown();
            Log.Information(string.Format("Service \"{0}\" has shutdown", ServiceName));
        }
    }
}
#endif