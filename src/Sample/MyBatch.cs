using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XecMe.Core.Batch;
using System.Diagnostics;
using XecMe.Core.Tasks;

namespace Sample
{
    public class MyBatch : IBatchProcess
    {
        void IBatchProcess.PreProcess()
        {
            Trace.WriteLine("Batch Preprocessor");
        }

        void IBatchProcess.Process()
        {
            TaskManager.Start(new TaskManagerConfig());
            TaskManager.WaitTasksToComplete();
        }

        void IBatchProcess.PostProcess()
        {
            Trace.WriteLine("Batch Preprocessor");
        }

        void IBatchProcess.Exception(Exception e)
        {
            Trace.WriteLine("Batch Preprocessor");
        }
    }
}
