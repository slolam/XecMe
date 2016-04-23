#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file IBatchProcess.cs is part of XecMe.
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

namespace XecMe.Core.Batch
{
    /// <summary>
    /// Batch job should implement this interface for creating the batch job.
    /// </summary>
    public interface IBatchProcess
    {
        /// <summary>
        /// This method is invoked by the framework in the begining to do initialization 
        /// required for the batch
        /// </summary>
        void PreProcess();
        /// <summary>
        /// This method is invoked after the PreProcess. This method should implement 
        /// the actual task the batch does.
        /// </summary>
        void Process();
        /// <summary>
        /// This method is invoked at the end of the task for the clean up.
        /// </summary>
        void PostProcess();

        /// <summary>
        /// This method is called by the framework in case of exception. It can be used for raising IMR
        /// </summary>
        /// <param name="e">Unhandled exception throw by the batch process</param>
        void Exception(Exception e);
    }
}
