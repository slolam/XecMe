#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file IService.cs is part of XecMe.
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

namespace XecMe.Core.Services
{
    /// <summary>
    /// IService is the interface to be implmented by a class to make itself hosted as a Windows Service
    /// </summary>
    public interface IService
    {
        string ServiceName { get; set; }
        /// <summary>
        /// Gets a value indicating whether the service can be paused and resumed.
        /// </summary>
        bool CanPauseAndContinue { get; }

        /// <summary>
        /// When implemented in a derived class, System.ServiceProcess.ServiceBase.OnContinue()
        /// runs when a Continue command is sent to the service by the Service Control
        /// Manager (SCM). Specifies actions to take when a service resumes normal functioning
        /// after being paused.
        /// </summary>
        void OnContinue();

        /// <summary>
        /// When implemented in a derived class, executes when a Pause command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service pauses.
        /// </summary>
        void OnPause();

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent
        /// to the service by the Service Control Manager (SCM). Specifies actions to
        /// take when a service stops running.
        /// </summary>
        void OnStop();

        /// <summary>
        /// When implemented in a derived class, executes when the system is shutting
        /// down. Specifies what should occur immediately prior to the system shutting
        /// down.
        /// </summary>
        void OnShutdown();

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent
        /// to the service by the Service Control Manager (SCM) or when the operating
        /// system starts (for a service that starts automatically). Specifies actions
        /// to take when the service starts.
        /// </summary>
        void OnStart();
    }
}
