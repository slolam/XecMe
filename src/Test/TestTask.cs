using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Test
{
    public class TestTask: ITask
    {
        private static int _count = -1, _delay;
        object _sync = new object();

        void ITask.OnStart(ExecutionContext context)
        {
            if (_count < 0)
            {
                int.TryParse(context.Parameters["count"]?.ToString(), out _count);
                //int.TryParse(context.Parameters["delay"]?.ToString(), out _delay);
            }
        }

        void ITask.OnStop(ExecutionContext context)
        {
            
        }

        void ITask.OnUnhandledException(Exception e)
        {
            Console.Error.WriteLine(e);
        }

        ExecutionState ITask.OnExecute(ExecutionContext context)
        {
            try
            {
                //var val = (int)context["EventArgs"];
                _count--;
                System.Threading.Thread.Sleep(10);
                Console.WriteLine($"Value {_count}");

                return _count < 0 ? ExecutionState.Stop : _count % 10 == 0? ExecutionState.Recycle :  ExecutionState.Executed;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
            return ExecutionState.Executed;
        }
    }
}
