using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Net;
using System.Net.NetworkInformation;

namespace XecMe.Core.Utils
{
    /// <summary>
    /// Static class represents the information of the service
    /// </summary>
    public static class ServiceInfo
    {
        static ServiceInfo()
        {
            try
            {
                MachineName = Environment.MachineName.ToUpper();
            }
            catch 
            {
                MachineName = string.Empty;
            }
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
        /// <summary>
        /// This is the timeout for the batch jobs
        /// </summary>
        public static int Timeout = -1;
    }
}
