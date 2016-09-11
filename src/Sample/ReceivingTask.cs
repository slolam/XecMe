using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XecMe.Core.Tasks;

namespace Sample
{
    public class ReceivingTask: ITask
    {
        void ITask.OnStart(ExecutionContext context)
        {
            Console.WriteLine("On Start called");
        }

        void ITask.OnStop(ExecutionContext context)
        {
            Console.WriteLine("On Stop called");
        }

        void ITask.OnUnhandledException(Exception e)
        {
            Console.WriteLine("On Exception called");
        }

        ExecutionState ITask.OnExecute(ExecutionContext context)
        {
            Console.WriteLine("On Execution called " + context["Counter"].ToString());
            System.Threading.Thread.Sleep(200);
            return ExecutionState.Executed;
        }
    }
}
