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
                int.TryParse(context.Parameters["delay"]?.ToString(), out _delay);
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
            System.Threading.Thread.Sleep(_delay);
            lock(_sync)
            {
                _count--;
                Console.WriteLine(string.Format("Executing {0}", _count));
            }

            if (_count < 0)
                return ExecutionState.Idle;

            return ExecutionState.Executed;
        }
    }
}
