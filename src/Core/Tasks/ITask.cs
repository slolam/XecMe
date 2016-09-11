using System;
using System.Collections.Generic;
using System.Text;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// ITask interface should be implemented by all the task in Cuso
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// This method is called by the framework. This method should be used to initialization of the task.
        /// </summary>
        /// <remarks>
        /// For the MQ poller kind of task, initialization of the queue should be done in this method.
        /// </remarks>
        void OnStart(ExecutionContext context);
        /// <summary>
        /// This method should be used to cleanup the task before the task is released.
        /// </summary>
        void OnStop(ExecutionContext context);

        /// <summary>
        /// This method is called back when an unhandled exception in OnExecute method occurs.
        /// </summary>
        void OnUnhandledException(Exception e);

        /// <summary>
        /// This method is invoked by the framework.
        /// </summary>
        ExecutionState OnExecute(ExecutionContext context);
    }


    /// <summary>
    /// State of the execution of ITask implementation
    /// </summary>
    public enum ExecutionState
    {
        /// <summary>
        /// The task did not perform any task due to no work
        /// </summary>
        Idle,
        /// <summary>
        /// Completed the task
        /// </summary>
        Executed,
        /// <summary>
        /// Stop the current instance of the Task and release it.
        /// </summary>
        Stop,
        /// <summary>
        /// Release the current instance and recreate new instance for next cycle.
        /// </summary>
        /// <remarks>In case the current instance having some unrecoverable problem like connection lost</remarks>
        Recycle,
        /// <summary>
        /// Exception while executing the task
        /// </summary>
        Exception
    }
}
