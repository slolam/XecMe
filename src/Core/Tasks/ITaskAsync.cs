using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// ITask interface should be implemented by all the task
    /// </summary>
    public interface ITaskAsync
    {
        /// <summary>
        /// This method is called by the framework. This method should be used to initialization of the task.
        /// </summary>
        /// <remarks>
        /// For the MQ poller kind of task, initialization of the queue should be done in this method.
        /// </remarks>
        Task OnStart(ExecutionContext context);
        /// <summary>
        /// This method should be used to cleanup the task before the task is released.
        /// </summary>
        Task OnStop(ExecutionContext context);

        /// <summary>
        /// This method is called back when an unhandled exception in OnExecute method occurs.
        /// </summary>
        Task OnUnhandledException(Exception e);

        /// <summary>
        /// This method is invoked by the framework.
        /// </summary>
        Task<ExecutionState> OnExecute(ExecutionContext context);
    }
}
