#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file ServiceInfo.cs is part of XecMe.
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
using System.ServiceProcess;
using System.Net;

namespace XecMe.Core.Utils
{
    public static class ServiceInfo
    {
        static ServiceInfo()
        {
            MachineName = Dns.GetHostName().ToUpper();
        }
        /// <summary>
        /// This is the name of the machine it is running on.
        /// </summary>
        public readonly static string MachineName;
        /// <summary>
        /// Name of the Windows Service/Batch being installed and run. 
        /// It is also used as reference to store the configuration in CUSO DB
        /// </summary>
        public static string ServiceName;
        /// <summary>
        /// Path where the Service is implemented
        /// </summary>
        public static string Directory;
        /// <summary>
        /// Account used for the Service
        /// </summary>
        public static ServiceAccount Account;
        /// <summary>
        /// Configuration file path for the Service
        /// </summary>
        public static string ConfigFilePath;
        /// <summary>
        /// CPU usage limiter
        /// </summary>
        public static float CpuUsageLimit;
        /// <summary>
        /// User identity for Service installation
        /// </summary>
        public static string User;
        /// <summary>
        /// Service start mode type.
        /// </summary>
        public static ServiceStartMode StartType = ServiceStartMode.Automatic;
    }
}
