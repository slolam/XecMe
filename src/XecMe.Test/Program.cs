using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskManager.Bootstrap = c => c.Verify();
            TaskManager.Start(new TaskManagerConfig());
            TaskManager.WaitTasksToComplete();
        }
    }
}
