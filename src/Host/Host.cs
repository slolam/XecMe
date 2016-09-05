#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file Host.cs is part of XecMe.
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
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using System;
using System.Configuration.Install;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using XecMe;
using XecMe.Installer;
using XecMe.Core.Utils;
using XecMe.Core;
using XecMe.Core.Configuration;
using XecMe.Common;
using XecMe.Core.Events;
using XecMe.Common.System;

namespace XecMe
{
    /// <summary>
    /// This class is the service host whether hosted as standalone process or Windows Service
    /// </summary>
    static class Host
    {
        #region Const
        public const string APP_NAME = "WinSvc";
        #endregion

        /// <summary> 
        /// This method  is entry point in WinSvc component 
        /// </summary> 
        /// <param name="a">Accepts string array as parameter and command line arguments are 
        /// passed to this parameter</param> 
        /// <returns></returns> 
        [MTAThread]
        public static int Main(string[] a)
        {
            Arguments args = new Arguments(a);
            if (!string.IsNullOrEmpty(args["?"])
                || !string.IsNullOrEmpty(args["help"]))
            {
                Usage();
                return 0;
            }

            //if(a.Length == 0)
            //{
            //    Application.EnableVisualStyles();
            //    Application.SetCompatibleTextRenderingDefault(false);
            //    Application.Run(new MainForm());
            //    return 0;
            //}

            if (!string.IsNullOrEmpty(args[Constants.PARAM_CPU]))
            {
                if (float.TryParse(args[Constants.PARAM_CPU], out ServiceInfo.CpuUsageLimit))
                {
                    if (ServiceInfo.CpuUsageLimit < 5 || ServiceInfo.CpuUsageLimit > 90)
                    {
                        Console.Write("The CPU uages limit should be between 5 and 90");
                        return -1;
                    }
                }
            }

            #region Service installation
            bool install = !string.IsNullOrEmpty(args[Constants.PARAM_INSTALL]),
                uninstall = !string.IsNullOrEmpty(args[Constants.PARAM_UNINSTALL]);
            if (install || uninstall)
            {
                if (string.IsNullOrEmpty(args[Constants.PARAM_NAME]))
                {
                    Console.WriteLine("Service Name is not supplied to install/uninstall the windows Service");
                    return -1;
                }
                ServiceInfo.ServiceName = args[Constants.PARAM_NAME];
                if (install && string.IsNullOrEmpty(args[Constants.PARAM_PATH]))
                {
                    Console.WriteLine("Path was not supplied to install the windows Service");
                    return -1;
                }
                ServiceInfo.Directory = args[Constants.PARAM_PATH];
                if (install && string.IsNullOrEmpty(args[Constants.PARAM_APPCONFIG]))
                {
                    Console.WriteLine("Application config path was not supplied");
                    return -1;
                }
                ServiceInfo.ConfigFilePath = args[Constants.PARAM_APPCONFIG];

                TransactedInstaller installer = new TransactedInstaller();
                Assembly asm = Assembly.GetExecutingAssembly();
                installer.Context = new InstallContext("WinSvc.log", a);
                AssemblyInstaller assemInstaller = new AssemblyInstaller(Assembly.GetExecutingAssembly(), a);
                Hashtable hash = new Hashtable();
                ServiceProcessInstaller spi;
                ServiceInstaller si;
                installer.Installers.Add(assemInstaller);
                //Supress the output of the installation 
                if (string.IsNullOrEmpty(args[Constants.PARAM_SUPRESS]))
                    Console.SetOut(TextWriter.Null);
                try
                {
                    if (install)
                    {
                        string assemblyPath;
                        if (ServiceInfo.CpuUsageLimit > 0)
                        {
                            assemblyPath = string.Format("\"{0}\" -s  -p=\"{1}\" -c=\"{2}\" -n=\"{3}\" -cpu=\"{4}\" ",
                            asm.Location, Path.GetFullPath(ServiceInfo.Directory), Path.GetFullPath(ServiceInfo.ConfigFilePath), ServiceInfo.ServiceName, ServiceInfo.CpuUsageLimit);
                        }
                        else
                        {
                            assemblyPath = string.Format("\"{0}\" -s  -p=\"{1}\" -c=\"{2}\" -n=\"{3}\" ",
                            asm.Location, Path.GetFullPath(ServiceInfo.Directory), Path.GetFullPath(ServiceInfo.ConfigFilePath), ServiceInfo.ServiceName);
                        }
                        string account = args[Constants.PARAM_SERVICEACCOUNT];
                        if (string.IsNullOrEmpty(account))
                        {
                            ServiceInfo.Account = ServiceAccount.NetworkService;
                        }
                        else
                        {
                            ServiceInfo.User = args[Constants.PARAM_USER];
                            switch (account.ToLower())
                            {
                                case "networkservice":
                                    ServiceInfo.Account = ServiceAccount.NetworkService;
                                    break;
                                case "localsystem":
                                    ServiceInfo.Account = ServiceAccount.LocalSystem;
                                    break;
                                case "localservice":
                                    ServiceInfo.Account = ServiceAccount.LocalService;
                                    break;
                                case "user":
                                    ServiceInfo.Account = ServiceAccount.User;
                                    if (string.IsNullOrEmpty(ServiceInfo.User))
                                        ServiceInfo.User = Thread.CurrentPrincipal.Identity.Name;
                                    break;
                                default:
                                    Console.WriteLine();
                                    Console.WriteLine("Invalid account for the service.");
                                    Console.WriteLine("Account can be either of LocalSystem/LocalService"
                                        + "/NetworkService/User");
                                    return -1;
                            }
                        }
                        string startType = args[Constants.PARAM_START_TYPE];
                        if (string.IsNullOrEmpty(startType))
                        {
                            ServiceInfo.StartType = ServiceStartMode.Automatic;
                        }
                        else
                        {
                            switch (startType.ToLower())
                            {
                                case "automatic":
                                    ServiceInfo.StartType = ServiceStartMode.Automatic;
                                    break;
                                case "manual":
                                    ServiceInfo.StartType = ServiceStartMode.Manual;
                                    break;
                                case "disable":
                                    ServiceInfo.StartType = ServiceStartMode.Disabled;
                                    break;
                                default:
                                    Console.WriteLine();
                                    Console.WriteLine("Invalid Service start mode");
                                    Console.WriteLine("Service start mode can be either of Automatic/Manual/Disabled");
                                    return -1;
                            }
                        }

                        assemInstaller.BeforeInstall += delegate(object sender, InstallEventArgs e)
                        {

                            spi = (ServiceProcessInstaller)assemInstaller.Installers[0]
                                .Installers[0];
                            si = (ServiceInstaller)assemInstaller.Installers[0].Installers[1];
                            spi.Account = ServiceInfo.Account;
                            spi.Username = ServiceInfo.User;
                            si.Description = string.Format("{0} is a .NET Shell Windows Service "
                            + "to run any service implemented using "
                            + "XecMe.Core.IService",
                            ServiceInfo.ServiceName);
                            si.DisplayName = ServiceInfo.ServiceName;
                            si.ServiceName = ServiceInfo.ServiceName;
                            si.StartType = ServiceInfo.StartType;
                        };
                        installer.Install(hash);
                        //Registry Path HKEY_LOCAL_MACHINE-> SYSTEM-> CONTROLSET*-> SERVICES -><NAME OF SERVICE> 
                        Registry.SetValue(string.Format(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\{0}\",
                            ServiceInfo.ServiceName), "ImagePath", assemblyPath, RegistryValueKind.String);

                        if (ServiceInfo.CpuUsageLimit > 0)
                        {
                            //This is required to support the Process name for the PerformanceCounters
                            //Registry Path HKEY_LOCAL_MACHINE-> SYSTEM-> CONTROLSET*-> SERVICES-> PerfProc-> Performance
                            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\PerfProc\Performance",
                                "ProcessNameFormat", 2, RegistryValueKind.DWord);
                        }

                        EventLog.WriteEntry(ServiceInfo.ServiceName, "Service installed successfully.",
                            EventLogEntryType.Information);
                    }
                    else
                    {
                        installer.Uninstall(null);
                        EventLog.WriteEntry(ServiceInfo.ServiceName, "Service uninstalled successfully.",
                            EventLogEntryType.Information);
                    }
                }
                catch (Exception e)
                {
                    EventLog.WriteEntry(ServiceInfo.ServiceName, string.Concat("Error while installing/uninstalling ",
                    ServiceInfo.ServiceName, e), EventLogEntryType.Error);
                }
                return 0;
            }
            #endregion Service installation

            ServiceInfo.Directory = args[Constants.PARAM_PATH];
            if (string.IsNullOrEmpty(ServiceInfo.Directory)
                || !Directory.Exists(ServiceInfo.Directory))
            {
                Console.WriteLine("Path was not supplied or does not exist for executing windows Service");
                return -1;
            }
            ServiceInfo.ConfigFilePath = args[Constants.PARAM_APPCONFIG];
            if (string.IsNullOrEmpty(ServiceInfo.ConfigFilePath)
                || !File.Exists(ServiceInfo.ConfigFilePath))
            {
                Console.WriteLine("Config path was not supplied or does not exist for executing windows Service");
                return -1;
            }
            if (string.IsNullOrEmpty(args[Constants.PARAM_NAME]))
            {
                Console.WriteLine("Batch/Service Name was not supplied");
                return -1;
            }
            ServiceInfo.ServiceName = args[Constants.PARAM_NAME];

            if (!string.IsNullOrEmpty(args[Constants.PARAM_TIMEOUT]))
            {
                int.TryParse(args[Constants.PARAM_TIMEOUT], out ServiceInfo.Timeout);
            }

            if (ServiceInfo.CpuUsageLimit >= 5 && ServiceInfo.CpuUsageLimit <= 90)
            {
                CpuUsageLimiter.Limit = ServiceInfo.CpuUsageLimit;
                CpuUsageLimiter.Start();
            }

            if (string.IsNullOrEmpty(args[Constants.PARAM_SERVICE]))
            {
                RunBatchProcess(a);
                return 0;
            }
            else
            {
                RunAsService();
                return 0;
            }
        }
        /// <summary> 
        /// This method will output the Usage text to the console 
        /// </summary> 
        private static void Usage()
        {
            Console.WriteLine("Usage: XecMeHost <command> [<options>]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine();
            Console.WriteLine("\t/?");
            Console.WriteLine("\tDisplays the help");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("\t/v");
            Console.WriteLine("\tVerbose");
            Console.WriteLine();
            Console.WriteLine("\t-p=<Path of the application> -c=<Path of batch config> [-t=<timeout for batch before it abort>] [-v]");
            Console.WriteLine("\tThis is a console mode for batch jobs");
            Console.WriteLine();
            Console.WriteLine("\t-i -n=<Name of the Windows Service> -p=<Path of the application>");
            Console.WriteLine("\t-c=<Path of application config> [-cpu=<Percent use between 5 and 90>]");
            Console.WriteLine("\t[-a=LocalService|LocalSystem|NetworkService|User -id=<Account Name>]");
            Console.WriteLine("\t[-m=Automatic|Manual|Disabled][-v]");
            Console.WriteLine("\tInstalls the application in the given path as Windows Service");
            Console.WriteLine("\twith the given name. By default the service will be installed as");
            Console.WriteLine("\tNetwork Service.");
            Console.WriteLine();
            Console.WriteLine("\t/u -n=<Name of the Service> [-v]");
            Console.WriteLine("\tUninstalls the Windows Service with the given name if installed thru XecMeHost.");
            Console.WriteLine();
        }


        /// <summary>
        /// Runs as a standalone console process.
        /// </summary>
        internal static void RunBatchProcess(string[] args)
        {
            AppDomainSetup appInfo = new AppDomainSetup();
            AppDomain current = AppDomain.CurrentDomain;

            appInfo.ConfigurationFile = ServiceInfo.ConfigFilePath;
            appInfo.AppDomainInitializer = new AppDomainInitializer(DomainInitializer.RunBatch);
            appInfo.AppDomainInitializerArguments = new string[] { ServiceInfo.ServiceName };
            appInfo.LoaderOptimization = LoaderOptimization.MultiDomain;
            appInfo.ApplicationBase = Path.GetFullPath(ServiceInfo.Directory);
            appInfo.PrivateBinPath = Path.GetFullPath(ServiceInfo.Directory);
            appInfo.ApplicationName = current.SetupInformation.ApplicationName;
            appInfo.DisallowCodeDownload = true;

            AppDomain app = null;
            Thread thread = new Thread(new ThreadStart(delegate()
                {
                    app = AppDomain.CreateDomain(ServiceInfo.ServiceName, current.Evidence, appInfo);
                }));
            thread.IsBackground = true;
            thread.Start();

            if (ServiceInfo.Timeout > 0)
                thread.Join(ServiceInfo.Timeout * 1000);
            else
                thread.Join();
        }

        /// <summary>
        /// Runs the current process as service when it is triggered from SCM framework.
        /// </summary>
        internal static void RunAsService()
        {
            AppDomainSetup appInfo = new AppDomainSetup();
            AppDomain current = AppDomain.CurrentDomain;

            appInfo.ConfigurationFile = ServiceInfo.ConfigFilePath;
            appInfo.AppDomainInitializer = new AppDomainInitializer(DomainInitializer.RunService);
            appInfo.AppDomainInitializerArguments = new string[] { ServiceInfo.ServiceName };
            appInfo.LoaderOptimization = LoaderOptimization.MultiDomain;
            appInfo.ApplicationBase = Path.GetFullPath(ServiceInfo.Directory);
            appInfo.PrivateBinPath = Path.GetFullPath(ServiceInfo.Directory);
            appInfo.ApplicationName = current.SetupInformation.ApplicationName;
            appInfo.DisallowCodeDownload = true;

            AppDomain app = AppDomain.CreateDomain(ServiceInfo.ServiceName, current.Evidence, appInfo);
        }
    }
}