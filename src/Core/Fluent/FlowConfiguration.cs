using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XecMe.Core.Tasks;

namespace XecMe.Core.Fluent
{
    public class FlowConfiguration : ITaskManagerConfig
    {
        public FlowConfiguration()
        {
            InternalRunners = new List<TaskRunner>();
            
        }
        public ICollection<TaskRunner> Runners
        {
            get
            {
                return InternalRunners;
            }
        }

        internal List<TaskRunner> InternalRunners
        {
            get; private set;
        }

        public IParallelTaskBuilder ParallelTask<TTaskType>(string name) where TTaskType: ITask
        {
            return new ParallelTaskBuilder(this, name, typeof(TTaskType));
        }

        public IEventTaskBuilder EventTask<TTaskType>(string name, Type taskType) where TTaskType : ITask
        {
            return new EventTaskBuilder(this, name, typeof(TTaskType));
        }

        public IRunOnceTaskBuilder RunOnce<TTaskType>(string name, Type taskType) where TTaskType : ITask
        {
            return new RunOnceTaskBuilder(this, name, typeof(TTaskType));
        }

        public IScheduledTaskBuilder ScheduledTask<TTaskType>(string name, Type taskType) where TTaskType : ITask
        {
            return new ScheduledTaskBuilder(this, name, typeof(TTaskType));
        }

        public ICronTaskBuilder CronTask<TTaskType>(string name, Type taskType) where TTaskType : ITask
        {
            return new CronTaskBuilder(this, name, typeof(TTaskType));
        }
    }
}
