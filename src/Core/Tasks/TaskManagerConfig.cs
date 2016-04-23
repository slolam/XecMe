#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file TaskManagerConfig.cs is part of XecMe.
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
using XecMe.Core.Configuration;

namespace XecMe.Core.Tasks
{
    public class TaskManagerConfig : ITaskManagerConfig
    {
        List<TaskRunner> _runners;
        public TaskManagerConfig()
        {
            _runners = new List<TaskRunner>();
            foreach (TaskRunnerElement taskRunner in TaskManagerSection.ThisSection.TaskRunners)
            {
                _runners.Add(taskRunner.GetRunner());
            }
        }

        #region ITaskManagerConfig Members

        ICollection<TaskRunner> ITaskManagerConfig.Runners
        {
            get { return _runners.AsReadOnly(); }
        }

        #endregion
    }
}
