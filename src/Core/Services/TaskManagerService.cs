#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TaskManagerService.cs is part of XecMe.
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
