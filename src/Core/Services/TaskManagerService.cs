#if !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Core.Tasks;
using XecMe.Configuration;
using XecMe.Core.Utils;
using XecMe.Common;

namespace XecMe.Core.Services
{
    /// <summary>
    /// This class can be used for as simple task manager
    /// </summary>
    public class TaskManagerService: IService
    {
        /// <summary>
        /// Name of the window service
        /// </summary>
        private string _serviceName;
#region IService Members

        /// <summary>
        /// Returns false indicating the service can be paused 
        /// </summary>
        bool IService.CanPauseAndContinue
        {
            get { return false; }
        }

        /// <summary>
        /// This method is not implemented and will throw an exception
        /// </summary>
        void IService.OnContinue()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This methis is not implemented and will throw an exception
        /// </summary>
        void IService.OnPause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the window service
        /// </summary>
        void IService.OnStop()
        {
            TaskManager.Stop();
        }

        /// <summary>
        /// Stops the windows service
        /// </summary>
        void IService.OnShutdown()
        {
            TaskManager.Stop();
        }

        /// <summary>
        /// Starts the windows service
        /// </summary>
        void IService.OnStart()
        {
            ExtensionElement extnElement = ExtensionsSection.ThisSection.Settings["ITaskManagerConfig"];

            string configTypeString = null;
            
            if (extnElement != null)
                configTypeString = extnElement.Type;

            ITaskManagerConfig tmConfig;
            if (!string.IsNullOrEmpty(configTypeString))
            {
                tmConfig = Reflection.CreateInstance<ITaskManagerConfig>(configTypeString);
            }
            else
            {
                tmConfig = new TaskManagerConfig();
            }
            TaskManager.Start(tmConfig);
        }

        /// <summary>
        /// Gets or sets service name
        /// </summary>
        string IService.ServiceName
        {
            get
            {
                return _serviceName;
            }
            set
            {
                _serviceName = value;
            }
        }

#endregion
    }
}
#endif
