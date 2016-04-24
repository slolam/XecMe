using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using XecMe.Core.Tasks;
using XecMe.Common.Diagnostics;

namespace XecMe.Core.Batch
{
    /// <summary>
    /// Simple batch process implementation that can be used for batch jobs
    /// </summary>
    public class TaskManagerBatch : IBatchProcess
    {
        /// <summary>
        /// This method is invoked by the framework. This method is used for the initializing job
        /// </summary>
        void IBatchProcess.PreProcess()
        {
            Log.Information("Batch job is Preprocess");
        }

        /// <summary>
        /// This method is invoked by the framework. This method is used for doing the batch job function
        /// </summary>
        void IBatchProcess.Process()
        {
            Log.Information("Batch job is Process");
            TaskManager.Start(new TaskManagerConfig());
            TaskManager.WaitTasksToComplete();
            Log.Information("Batch job is Process");
        }

        /// <summary>
        /// This methid is invoked by the framework and should implement any cleanup
        /// </summary>
        void IBatchProcess.PostProcess()
        {
            Log.Information("Batch job is Postprocess");
        }

        /// <summary>
        /// This method is invoked by the framework if there is an unhandled exception been thrown by the batch job
        /// </summary>
        /// <param name="e">Unhandled exception been catch by the framework</param>
        void IBatchProcess.Exception(Exception e)
        {
            Log.Error(string.Format("Exception occured: {0}", e));
        }
    }
}
