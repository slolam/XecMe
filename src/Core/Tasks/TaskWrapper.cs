using System;
using System.Collections.Generic;
using System.Text;
using XecMe.Core.Utils;
using XecMe.Common;
using System.Diagnostics;
using System.Threading;
using XecMe.Core.Events;
using System.Reflection;

namespace XecMe.Core.Tasks
{
    /// <summary>
    /// Task wrapper that abstracts the task from runner
    /// </summary>
    internal class TaskWrapper
    {
        private static readonly Type AsyncProviderGenericType = typeof(AsyncTaskProvider<>);
        /// <summary>
        /// 
        /// </summary>
        private bool _initialized;
        /// <summary>
        /// Stop watch to monitor the execurion time
        /// </summary>
        private Stopwatch _stopwatch = new Stopwatch();

        /// <summary>
        /// The container scope
        /// </summary>
        private IDisposable _containerScope;

        /// <summary>
        /// Creates instance of this class
        /// </summary>
        /// <param name="taskType">Instance of the task implementing ITask interface</param>
        /// <param name="executionContext">Execution context</param>
        public TaskWrapper(Type taskType, ExecutionContext executionContext)
        {
            taskType.NotNull(nameof(taskType));
            executionContext.NotNull(nameof(executionContext));
            Context = executionContext;
            TaskType = taskType;
        }

        /// <summary>
        /// Gets the type of the task.
        /// </summary>
        /// <value>
        /// The type of the task.
        /// </value>
        public Type TaskType { get; private set; }

        /// <summary>
        /// Instance of the task.
        /// </summary>
        /// <value>
        /// The task.
        /// </value>
        public ITask Task { get; private set; }

        /// <summary>
        /// Initializes the specified execution context.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        private void Initialize(ExecutionContext executionContext = null)
        {
            if (!_initialized)
            {
                try
                {
                    var task = executionContext.Container.GetInstance(TaskType);
                    if(task is ITaskAsync)
                    {
                        Task = AsyncProviderGenericType.MakeGenericType(task.GetType()).InvokeMember(".ctor", BindingFlags.CreateInstance, null, null, new object[] { task }) as ITask;
                    }
                    else
                    {
                        Task = task as ITask;
                    }
                    Task.OnStart(executionContext);
                    _initialized = true;
                    executionContext.TaskRunner.TraceInformation("Started");
                }
                catch (Exception e)
                {
                    try
                    {
                        executionContext.TaskRunner.TraceError("Caught unhandled exception in OnStart, calling OnUnhandledException: {0}", e);
                        Task.OnUnhandledException(e);
                    }
                    catch (Exception badEx)
                    {
                        executionContext.TaskRunner.TraceError("Bad Error: {0}", badEx);
                    }
                }
            }
        }

        /// <summary>
        /// Runs the task. If the task is not initialized, it will intialize the task and then run it
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        /// <returns></returns>
        public ExecutionState RunTask(ExecutionContext executionContext = null)
        {
            if (executionContext == null)
                executionContext = Context;

            using (var scope = executionContext.Container.BeginScope())
            {
                Initialize(executionContext);
                try
                {
                    executionContext.TaskRunner.TraceInformation("Executing on Managed thread {0}", Thread.CurrentThread.ManagedThreadId);
                    _stopwatch.Restart();
                    ExecutionState state = ExecutionState.Idle;
                    state = Task.OnExecute(executionContext);
                    _stopwatch.Stop();
                    executionContext.TaskRunner.TraceInformation("Executed in {2} ms. with a return value {0} on Managed thread {1}", state, Thread.CurrentThread.ManagedThreadId, _stopwatch.ElapsedMilliseconds);
                    if (state == ExecutionState.Recycle
                        || state == ExecutionState.Stop)
                    {
                        InternalRelease(executionContext);
                    }
                    return state;
                }
                catch (Exception ex)
                {
                    _stopwatch.Stop();
                    executionContext.TaskRunner.TraceInformation("Executed in {2} ms. with a return value {0} on Managed thread {1}", ExecutionState.Exception, Thread.CurrentThread.ManagedThreadId, _stopwatch.ElapsedMilliseconds);
                    try
                    {
                        executionContext.TaskRunner.TraceError("Caught unhandled exception in OnExecute, calling OnUnhandledException: {0}", ex);
                        Task.OnUnhandledException(ex);
                    }
                    catch (Exception badEx)
                    {
                        executionContext.TaskRunner.TraceError("Bad Error: {0}", badEx);
                    }
                    return ExecutionState.Exception;
                }
            }
        }

        /// <summary>
        /// Releases this instance.
        /// </summary>
        public void Release(ExecutionContext executionContext = null)
        {
            if (executionContext == null)
                executionContext = Context;

            using (var scope = executionContext.Container.BeginScope())
            {
                InternalRelease(executionContext);
            }
        }

        /// <summary>
        /// Internal release the instances.
        /// </summary>
        /// <param name="executionContext">The execution context.</param>
        private void InternalRelease(ExecutionContext executionContext = null)
        {
            try
            {
                if (_initialized)
                    Task.OnStop(executionContext);
                executionContext.TaskRunner.TraceInformation("Stopped successfully");
            }
            catch (Exception e)
            {
                try
                {
                    executionContext.TaskRunner.TraceError("Caught unhandled exception in OnStop, calling OnUnhandledException: {0}", e);
                    Task.OnUnhandledException(e);
                }
                catch (Exception badEx)
                {
                    executionContext.TaskRunner.TraceError("Bad Error: {0}", badEx);
                }
            }
            _containerScope?.Dispose();
            _containerScope = null;
            _initialized = false;
        }

        /// <summary>
        /// Gets the task instance.
        /// </summary>
        /// <returns></returns>
        private ITask GetTaskInstance()
        {
            ITask task = ExecutionContext.InternalContainer.GetInstance(TaskType) as ITask;
            EventManager.Register(task);
            return task;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>
        /// The context.
        /// </value>
        public ExecutionContext Context { get; private set; }
    }
}
