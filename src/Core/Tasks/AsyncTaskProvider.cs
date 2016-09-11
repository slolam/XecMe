using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Common;

namespace XecMe.Core.Tasks
{
    class AsyncTaskProvider<TTask> : ITask
        where TTask : ITaskAsync
    {
        ITaskAsync _asyncTask;
        public AsyncTaskProvider(TTask asyncTask)
        {
            asyncTask.NotNull(nameof(asyncTask));

            _asyncTask = asyncTask;
        }
        ExecutionState ITask.OnExecute(ExecutionContext context)
        {
            return _asyncTask.OnExecute(context).Result;
        }

        void ITask.OnStart(ExecutionContext context)
        {
            _asyncTask.OnStart(context).Wait();
        }

        void ITask.OnStop(ExecutionContext context)
        {
            _asyncTask.OnStop(context).Wait();
        }

        void ITask.OnUnhandledException(Exception e)
        {
            _asyncTask.OnUnhandledException(e).Wait();
        }
    }
}
