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
        /// It is also used as reference to store the configuration in DB
        /// </summary>
        public static string ServiceName;
        /// <summary>
        /// Path where the Service is implemented
        /// </summary>
        public static string Directory;
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
#if !NETSTANDARD2_0
        /// <summary>
        /// Account used for the Service
        /// </summary>
        public static ServiceAccount Account;
        /// <summary>
        /// Service start mode type.
        /// </summary>
        public static ServiceStartMode StartType = ServiceStartMode.Automatic;
#endif
        /// <summary>
        /// This is the timeout for the batch jobs
        /// </summary>
        public static int Timeout = -1;
    }

#if !NETSTANDARD2_0
    /// <summary>
    /// Indicates the start mode of the service.
    /// </summary>
    public enum ServiceStartMode
    {
        /// <summary>
        /// Indicates that the service is to be started (or was started) by the operating
        ///     system, at system start-up. If an automatically started service depends on a
        ///     manually started service, the manually started service is also started automatically
        ///     at system startup.
        /// </summary>
        Automatic = 2,
        /// <summary>
        /// Indicates that the service is started only manually, by a user (using the Service
        ///     Control Manager) or by an application.
        /// </summary>
        Manual = 3,
        /// <summary>
        /// Indicates that the service is disabled, so that it cannot be started by a user
        ///     or application.
        /// </summary>
        Disabled = 4
    }

        
  /// <summary>
  /// 
  /// </summary>
    public enum ServiceAccount
    {
        /// <summary>
        /// An account that acts as a non-privileged user on the local computer, and presents
        ///     anonymous credentials to any remote server.
        /// </summary>
        LocalService = 0,
        /// <summary>
        /// An account that provides extensive local privileges, and presents the computer's
        ///     credentials to any remote server.
        /// </summary>
        NetworkService = 1,
        /// <summary>
        /// An account, used by the service control manager, that has extensive privileges
        ///     on the local computer and acts as the computer on the network.
        /// </summary>
        LocalSystem = 2,
        /// <summary>
        /// An account defined by a specific user on the network. Specifying User for the
        ///     System.ServiceProcess.ServiceProcessInstaller.Account member causes the system
        ///     to prompt for a valid user name and password when the service is installed, unless
        ///     you set values for both the System.ServiceProcess.ServiceProcessInstaller.Username
        ///     and System.ServiceProcess.ServiceProcessInstaller.Password properties of your
        ///     System.ServiceProcess.ServiceProcessInstaller instance.
        /// </summary>
        User = 3
    }
#endif
}
