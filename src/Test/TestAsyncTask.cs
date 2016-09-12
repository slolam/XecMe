using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Test
{
    public class TestAsyncTask: ITaskAsync
    {
        private static int _count = -1, _delay;
        object _sync = new object();

        async Task ITaskAsync.OnStart(ExecutionContext context)
        {
            if (_count < 0)
            {
                int.TryParse(context.Parameters["count"]?.ToString(), out _count);
                //int.TryParse(context.Parameters["delay"]?.ToString(), out _delay);
            }
        }

        async Task ITaskAsync.OnStop(ExecutionContext context)
        {
            
        }

        async Task ITaskAsync.OnUnhandledException(Exception e)
        {
            Console.Error.WriteLine(e);
        }

        async Task<ExecutionState> ITaskAsync.OnExecute(ExecutionContext context)
        {
            try
            {
                //var val = (int)context["EventArgs"];
                _count--;
                await Task.Delay(10);
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
