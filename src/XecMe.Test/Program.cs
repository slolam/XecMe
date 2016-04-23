using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Diagnostics;
using XecMe.Core.Tasks;

namespace XecMe.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.WarningSink = Console.WriteLine;
            Log.InformationSink = Console.WriteLine;
            Log.ErrorSink = Console.WriteLine;
            TaskManager.Bootstrap = c => c.Verify();
            TaskManager.Start(new TaskManagerConfig());
            TaskManager.WaitTasksToComplete();
        }
    }
}
