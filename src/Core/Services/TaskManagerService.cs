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
using System.Linq;
using System.Text;
using XecMe.Core.Tasks;
using XecMe.Configuration;
using XecMe.Core.Utils;
using XecMe.Common;

namespace XecMe.Core.Services
{
    public class TaskManagerService: IService
    {
        private string _serviceName;
        #region IService Members

        bool IService.CanPauseAndContinue
        {
            get { return false; }
        }

        void IService.OnContinue()
        {
            throw new NotImplementedException();
        }

        void IService.OnPause()
        {
            throw new NotImplementedException();
        }

        void IService.OnStop()
        {
            TaskManager.Stop();
        }

        void IService.OnShutdown()
        {
            TaskManager.Stop();
        }

        void IService.OnStart()
        {
            string configTypeString = ExtensionsSection.ThisSection.GetExtensions("configs")["ITaskManagerConfig"].Type;
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
