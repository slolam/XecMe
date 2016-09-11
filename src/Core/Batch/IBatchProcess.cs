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
