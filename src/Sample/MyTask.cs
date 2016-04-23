using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XecMe.Common;
using XecMe.Core.Tasks;
using System.Diagnostics;


namespace Sample
{
    public class MyTask : ITask
    {
        int i = 0;
        void ITask.OnStart(ExecutionContext context)
        {

        }

        void ITask.OnStop(ExecutionContext context)
        {
            
        }

        void ITask.OnUnhandledException(Exception e)
        {
            Trace.TraceError("Some error occured {0}", e.Message);
        }

        ExecutionState ITask.OnExecute(ExecutionContext context)
        {
            if (i < 100)
            {
                Console.WriteLine("Setting " + i.ToString());
                context["Counter"] = ++i;
                return ExecutionState.Executed;
            }
            return ExecutionState.Stop;
        }
    }
}
