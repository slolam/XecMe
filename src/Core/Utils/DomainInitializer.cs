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
using System.Linq;
using System.Text;
using System.Configuration;
using System.ServiceProcess;
using XecMe.Core.Services;
using XecMe.Core.Batch;
using XecMe.Common;

namespace XecMe.Core.Utils
{
    /// <summary>
    /// This class loads of the type the IService start the Windows Service
    /// </summary>
    public static class DomainInitializer
    {
#if DEBUG
        static IService service;
#endif
        public static void RunService(string[] args)
        {
            Initialize(args);
            string serviceTypeName = ConfigurationManager.AppSettings["IService"];

            
#if DEBUG
            service = Reflection.CreateInstance<IService>(serviceTypeName);
            service.OnStart();
            Console.ReadKey();
#else
            ServiceBase service = new ServiceHost(args[0], Reflection.CreateInstance<IService>(serviceTypeName));
            ServiceBase.Run(service);
#endif
        }

        public static void RunBatch(string[] args)
        {
            Initialize(args);
            string batchTypeName = ConfigurationManager.AppSettings["IBatchProcess"];

            IBatchProcess batchProcess = Reflection.CreateInstance<IBatchProcess>(batchTypeName);
            try
            {
                batchProcess.PreProcess();
                batchProcess.Process();
                batchProcess.PostProcess();
            }
            catch (Exception e)
            {
                ///TODO
                ///Logging the exception
                batchProcess.Exception(e);
            }
        }

        private static void Initialize(string[] args)
        {
            ServiceInfo.ServiceName = args[0];
        }
    }
}
