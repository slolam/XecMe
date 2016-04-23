#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file DomainInitializer.cs is part of XecMe.
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
using System.Configuration;
using System.ServiceProcess;
using XecMe.Core.Services;
using XecMe.Core.Batch;
using XecMe.Common;
using System.Diagnostics;
using XecMe.Configuration;
using System.IO;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Utils
{
    /// <summary>
    /// This class loads of the type the IService start the Windows Service
    /// </summary>
    public static class DomainInitializer
    {
        public static void RunService(string[] args)
        {
            Initialize(args);
            
            string serviceTypeName = ExtensionsSection.ThisSection.Settings["IService"].Type;

            if (Debugger.IsAttached)
            {
                IService service;
                service = Reflection.CreateInstance<IService>(serviceTypeName);
                service.OnStart();
                Console.ReadKey();
            }
            else
            {
                ServiceBase service = new ServiceHost(args[0], Reflection.CreateInstance<IService>(serviceTypeName));
                ServiceBase.Run(service);
            }
        }

        public static void RunBatch(string[] args)
        {
            Initialize(args);

            string batchTypeName = ExtensionsSection.ThisSection.Settings["IBatchProcess"].Type;

            IBatchProcess batchProcess = Reflection.CreateInstance<IBatchProcess>(batchTypeName);
            try
            {
                batchProcess.PreProcess();
                batchProcess.Process();
                batchProcess.PostProcess();
            }
            catch (Exception e)
            {
                try
                {
                    batchProcess.Exception(e);
                    Log.Error(string.Format("An unhandled exception was thrown by the batch job {0}", e));
                }
                catch (Exception badEx)
                {
                    Log.Error(string.Format("Bad Error: {0}", badEx));
                }
            }
        }

        private static void Initialize(string[] args)
        {
            ServiceInfo.ServiceName = args[0];
            AppDomain current = AppDomain.CurrentDomain;
            current.AssemblyLoad += new AssemblyLoadEventHandler(AssemblyLoad);
            current.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);
            current.ReflectionOnlyAssemblyResolve += new ResolveEventHandler(ReflectionOnlyAssemblyResolve);
            current.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
            Directory.SetCurrentDirectory(current.BaseDirectory);
        }

        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(string.Format("Unhandled Exception: {0}", e.ExceptionObject));
        }

        static System.Reflection.Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.Error(string.Format("Reflection Resolve assembly {0} : {1}", args.Name, args.RequestingAssembly));
            return null;
        }

        static System.Reflection.Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.Error(string.Format("Resolve assembly {0} : {1}", args.Name, args.RequestingAssembly));
            return null;
        }

        static void AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Log.Information(string.Format("Assembly loaded {0}", args.LoadedAssembly));
        }
    }
}
