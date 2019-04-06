using System;
#if !NETSTANDARD2_0
using XecMe.Core.Services;
#endif
using XecMe.Core.Batch;
using XecMe.Common;
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
#if !NETSTANDARD2_0
        /// <summary>
        /// Runs the service.
        /// </summary>
        /// <param name="args">The arguments.</param>
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
#endif
        /// <summary>
        /// Runs the batch.
        /// </summary>
        /// <param name="args">The arguments.</param>
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

        /// <summary>
        /// Initializes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
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

        /// <summary>
        /// Unhandleds the exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(string.Format("Unhandled Exception: {0}", e.ExceptionObject));
        }

        /// <summary>
        /// Reflections the only assembly resolve.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        static System.Reflection.Assembly ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.Error(string.Format("Reflection Resolve assembly {0} : {1}", args.Name, args.RequestingAssembly));
            return null;
        }

        /// <summary>
        /// Assemblies the resolve.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        static System.Reflection.Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.Error(string.Format("Resolve assembly {0} : {1}", args.Name, args.RequestingAssembly));
            return null;
        }

        /// <summary>
        /// Assemblies the load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AssemblyLoadEventArgs"/> instance containing the event data.</param>
        static void AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Log.Information(string.Format("Assembly loaded {0}", args.LoadedAssembly));
        }
    }
}
